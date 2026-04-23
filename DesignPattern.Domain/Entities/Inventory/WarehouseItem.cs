using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Order;
namespace DesignPattern.Domain.Entities.Inventory;

public class WarehouseItem : BaseEntity
{
  public Guid WarehouseId { get; private set; }
  public Name WarehouseName { get; private set; }
  public Guid ProductId { get; private set; }
  public Name ProductName { get; private set; }
  public StandardText Sku { get; private set; }
  public StandardText VariantGroupId { get; private set; }
  public Quantity Quantity { get; private set; }
  public Quantity WaitingForDelivery { get; private set; } = Quantity.Zero;

  private WarehouseItem(Guid warehouseId, Name warehouseName, Guid productId, Name productName, StandardText sku, StandardText variantGroupId, Quantity quantity)
  {
    WarehouseId = warehouseId;
    WarehouseName = warehouseName;
    ProductId = productId;
    ProductName = productName;
    Sku = sku;
    VariantGroupId = variantGroupId;
    Quantity = quantity;
  }
  public static Result<WarehouseItem> Create(Guid warehouseId, string warehouseName, Guid productId, string productName, string sku, string variantGroupId, int quantity)
  {
    var quantityResult = Quantity.Create(quantity);
    if (quantityResult.IsFailure)
      return quantityResult.Error;

    var warehouseNameResult = Name.Create(warehouseName);
    if (warehouseNameResult.IsFailure)
      return warehouseNameResult.Error;

    var productNameResult = Name.Create(productName);
    if (productNameResult.IsFailure)
      return productNameResult.Error;

    var skuResult = StandardText.Create(sku);
    if (skuResult.IsFailure)
      return skuResult.Error;

    var variantGroupIdResult = StandardText.Create(variantGroupId);
    if (variantGroupIdResult.IsFailure)
      return variantGroupIdResult.Error;

    return new WarehouseItem(warehouseId, warehouseNameResult.Value, productId, productNameResult.Value, skuResult.Value, variantGroupIdResult.Value, quantityResult.Value);
  }
  public Result Update(string? productName, string? sku, string? variantGroupId, int? updateQuantity)
  {
    if (productName is not null)
    {
      var nameResult = Name.Create(productName);
      if (nameResult.IsFailure)
        return nameResult.Error;
      ProductName = nameResult.Value;
    }
    if (sku is not null)
    {
      var skuResult = StandardText.Create(sku);
      if (skuResult.IsFailure)
        return skuResult.Error;
      Sku = skuResult.Value;
    }
    if (variantGroupId is not null)
    {
      var variantGroupIdResult = StandardText.Create(variantGroupId);
      if (variantGroupIdResult.IsFailure)
        return variantGroupIdResult.Error;
      VariantGroupId = variantGroupIdResult.Value;
    }
    if (updateQuantity is not null)
    {
      var quantityResult = Quantity.Create(updateQuantity.Value);
      if (quantityResult.IsFailure)
        return quantityResult.Error;
      if (quantityResult.Value < WaitingForDelivery)
        return WarehouseErrors.QuantityLessThanWaitingForDelivery;
      Quantity = quantityResult.Value;
    }
    return true;

  }

  public (InventoryAllocation allocation, Quantity Remaining)
    AddWarehouseItemWaitingForDelivery(Quantity requestQuantity)
  {
    var available = Quantity - WaitingForDelivery;

    if (requestQuantity > available)
    {
      // lấy hết phần còn lại
      WaitingForDelivery += available;

      var remaining = requestQuantity - available;
      var allocation = InventoryAllocation.Create(WarehouseId, WarehouseName, ProductId, ProductName, available);

      return (allocation, remaining);
    }
    else
    {
      WaitingForDelivery += requestQuantity;
      var allocation = InventoryAllocation.Create(WarehouseId, WarehouseName, ProductId, ProductName, requestQuantity);

      return (allocation, Quantity.Zero);
    }
  }
  public Quantity GetStock()
  {
    var availableStock = Quantity - WaitingForDelivery;
    return availableStock;
  }
}