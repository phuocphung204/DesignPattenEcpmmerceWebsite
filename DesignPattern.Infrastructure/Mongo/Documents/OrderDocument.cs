using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal sealed class OrderDocument : BaseDocument
{
  [BsonElement("userId")]
  [BsonRequired]
  public Guid UserId { get; init; }

  [BsonElement("shippingInfo")]
  public ShippingInfoDocument ShippingInfo { get; init; } = default!;

  [BsonElement("discountCode")]
  public string? DiscountCode { get; init; }

  [BsonElement("pointsUsed")]
  public int PointsUsed { get; init; } = 0;

  [BsonElement("subTotal")]
  public decimal SubTotal { get; init; }

  [BsonElement("shippingFee")]
  public decimal ShippingFee { get; init; }

  [BsonElement("discountAmount")]
  public decimal DiscountAmount { get; init; }

  [BsonElement("rewardPointsAmount")]
  public decimal RewardPointsAmount { get; init; }

  [BsonElement("grandAmount")]
  public decimal GrandAmount { get; init; }

  [BsonElement("paymentStatus")]
  [BsonRepresentation(BsonType.String)]
  public PaymentStatusEnum PaymentStatus { get; init; }

  [BsonElement("status")]
  [BsonRepresentation(BsonType.String)]
  public OrderStatusEnum Status { get; init; }

  [BsonElement("paymentInfo")]
  public PaymentInfoDocument PaymentInfo { get; init; } = default!;

  [BsonElement("note")]
  public string Note { get; init; } = string.Empty;

  [BsonElement("items")]
  public List<OrderItemDocument> Items { get; init; } = new();

  [BsonElement("histories")]
  public List<OrderHistoryDocument> Histories { get; init; } = new();

  [BsonElement("inventoryAllocations")]
  public List<InventoryAllocationDocument> InventoryAllocations { get; init; } = new();
}

// --- CÁC SUB-DOCUMENTS CỦA ORDER ---

internal sealed class ShippingInfoDocument
{
  [BsonElement("type")]
  [BsonRepresentation(BsonType.String)]
  public ShippingType Type { get; init; }

  [BsonElement("address")]
  public VietNamAddress Address { get; init; } = default!;
}
// internal sealed class VietNamShippingAddress : BaseDocument
// {
//   [BsonElement("receiverName")]
//   [BsonRequired]
//   public required string ReceiverName { get; init; }

//   [BsonElement("phoneNumber")]
//   [BsonRequired]
//   public required string PhoneNumber { get; init; }

//   [BsonElement("province")]
//   public string Province { get; init; } = string.Empty;

//   [BsonElement("district")]
//   public string District { get; init; } = string.Empty;

//   [BsonElement("ward")]
//   public string Ward { get; init; } = string.Empty;

//   [BsonElement("street")]
//   public string Street { get; init; } = string.Empty;

//   [BsonElement("provinceCode")]
//   public string ProvinceCode { get; init; } = string.Empty;

//   [BsonElement("districtCode")]
//   public string DistrictCode { get; init; } = string.Empty;

//   [BsonElement("wardCode")]
//   public string WardCode { get; init; } = string.Empty;

//   [BsonElement("country")]
//   public string Country { get; init; } = string.Empty;
// }

internal sealed class PaymentInfoDocument
{
  [BsonElement("type")]
  [BsonRepresentation(BsonType.String)]
  public PaymentType Type { get; init; } // Cash, BankTransfer, CreditCard

  [BsonElement("paymentProvider")]
  public string? PaymentProvider { get; init; }

  [BsonElement("paymentChannel")]
  public string? PaymentChannel { get; init; }

  [BsonElement("transactionId")]
  public string? TransactionId { get; init; }
  [BsonElement("paidAt")]
  public DateTime? PaidAt { get; init; }

  // Thông tin Bank
  [BsonElement("bankName")]
  public string? BankName { get; init; }

  [BsonElement("accountNumber")]
  public string? AccountNumber { get; init; }

  // Thông tin Credit Card
  [BsonElement("cardNumber")]
  public string? CardNumber { get; init; }

  [BsonElement("cardHolder")]
  public string? CardHolder { get; init; }

  [BsonElement("cardType")]
  public string? CardType { get; init; }
}

internal sealed class OrderItemDocument
{
  [BsonElement("productId")]
  public Guid ProductId { get; init; }

  [BsonElement("productName")]
  public string ProductName { get; init; } = string.Empty;

  [BsonElement("quantity")]
  public int Quantity { get; init; }

  [BsonElement("sku")]
  public string Sku { get; init; } = string.Empty;

  [BsonElement("sellingPrice")]
  public decimal SellingPrice { get; init; }

  [BsonElement("purchasePrice")]
  public decimal PurchasePrice { get; init; }

  [BsonElement("thumbnailUrl")]
  public string? ThumbnailUrl { get; init; }

  [BsonElement("attributes")]
  public List<AttributeItem> Attributes { get; init; } = new();

}

internal sealed class OrderHistoryDocument
{
  [BsonElement("status")]
  public OrderStatusEnum Status { get; init; }

  [BsonElement("statusChangedDate")]
  public DateTime StatusChangedDate { get; init; }

  [BsonElement("changedBy")]
  public Guid? ChangedBy { get; init; }

  [BsonElement("changedByRole")]
  [BsonRepresentation(BsonType.String)]
  public UserRole ChangedByRole { get; init; }

  [BsonElement("note")]
  public string Note { get; init; } = string.Empty;
}

internal sealed class InventoryAllocationDocument
{
  [BsonElement("warehouseId")]
  public Guid WarehouseId { get; init; }

  [BsonElement("warehouseName")]
  public string WarehouseName { get; init; } = string.Empty;

  [BsonElement("productId")]
  public Guid ProductId { get; init; }

  [BsonElement("productName")]
  public string ProductName { get; init; } = string.Empty;

  [BsonElement("allocated")]
  public int Allocated { get; init; }
}