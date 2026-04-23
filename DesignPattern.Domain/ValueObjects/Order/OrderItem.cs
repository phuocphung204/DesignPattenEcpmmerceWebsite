using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.ValueObjects.Order;

public class OrderItem
{
  // public ID OrderId { get; private set; }
  public Guid ProductId { get; private set; }
  public Name ProductName { get; private set; }
  public Quantity Quantity { get; private set; }
  public StandardText Sku { get; private set; }
  public Price SellingPrice { get; private set; } // Price at which the item is sold to the customer
  public Price PurchasePrice { get; private set; } // Price at which the item was purchased from the supplier
  // THÊM: Để hiển thị trong lịch sử mua hàng
  public string? ThumbnailUrl { get; private set; }
  public List<AttributeItem> Attributes { get; private set; } = new List<AttributeItem>();

  private OrderItem(
    Guid productId,
    Name productName,
    Quantity quantity,
    StandardText sku,
    Price sellingPrice,
    Price purchasePrice,
    string? thumbnailUrl,
    List<AttributeItem> attributes)
  {
    ProductId = productId;
    ProductName = productName;
    Quantity = quantity;
    Sku = sku;
    Attributes = attributes;
    SellingPrice = sellingPrice;
    PurchasePrice = purchasePrice;
    ThumbnailUrl = thumbnailUrl;
  }

  public static Result<OrderItem> Create(
    Guid productId,
    Name productName,
    int quantity,
    StandardText sku,
    Price sellingPrice,
    Price purchasePrice,
    string? thumbnailUrl,
    List<AttributeItem> attributes)
  {

    Result<Quantity> quantityResult = Quantity.Create(quantity);
    if (quantityResult.IsFailure)
      return quantityResult.Error;

    OrderItem orderItem = new OrderItem(
      productId,
      productName,
      quantityResult.Value,
      sku,
      sellingPrice,
      purchasePrice,
      thumbnailUrl,
      attributes);

    return orderItem;
  }
}

