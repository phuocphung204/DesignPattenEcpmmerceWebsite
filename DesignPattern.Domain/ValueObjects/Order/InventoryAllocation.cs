using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.ValueObjects.Order;

public record InventoryAllocation
{
  public Guid WarehouseId { get; init; }
  public Name WarehouseName { get; init; }
  public Guid ProductId { get; init; }
  public Name ProductName { get; init; }
  public Quantity Allocated { get; init; }
  private InventoryAllocation(
    Guid warehouseId,
    Name warehouseName,
    Guid productId,
    Name productName,
    Quantity allocated)
  {
    this.WarehouseId = warehouseId;
    this.WarehouseName = warehouseName;
    this.ProductId = productId;
    this.ProductName = productName;
    this.Allocated = allocated;
  }

  public static InventoryAllocation Create(Guid warehouseId, Name warehouseName, Guid productId, Name productName, Quantity allocated)
  {
    return new InventoryAllocation(warehouseId, warehouseName, productId, productName, allocated);
  }
}



