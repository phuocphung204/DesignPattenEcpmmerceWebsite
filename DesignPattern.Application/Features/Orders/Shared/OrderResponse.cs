using DesignPattern.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesignPattern.Application.Features.Orders.Shared;

public record OrderAttributeResponse(
  string Name,
  string Value);

public record InventorysAllocationResponse
{
  public Guid WarehouseId { get; set; }
  public string WarehouseName { get; set; }
  public Guid ProductId { get; set; }
  public string ProductName { get; set; }
  public int AllocatedQuantity { get; set; }
}
public record OrderHistoryResponse(
  OrderStatusEnum Status,
  DateTime StatusChangedDate,
  Guid? ChangedBy,
  UserRole ChangedByRole,
  string? Note);

public record ShippingAddressResponse(
  string ReceiverName,
  string ReceiverPhone,
  string Country,
  string Province,
  string District,
  string Ward,
  string Street,
  string ProvinceCode,
  string DistrictCode,
  string WardCode
);

public record ShippingInfoResponse(
  ShippingType Type, // Standard, Express
  ShippingAddressResponse ShippingAddress);

[JsonConverter(typeof(PaymentInfoResponseJsonConverter))]
public abstract record PaymentInfoResponse(
  PaymentType Type,
  string? PaymentProvider = null,
  string? PaymentChannel = null,
  string? TransactionId = null,
  DateTime? PaidAt = null
  );

public record CashPaymentInfoResponse(
  PaymentType Type, // Cash
  string? PaymentProvider = null,
  string? PaymentChannel = null,
  string? TransactionId = null,
  DateTime? PaidAt = null
) : PaymentInfoResponse(Type, PaymentProvider, PaymentChannel, TransactionId, PaidAt);

public record CreditCardPaymentInfoResponse(
  PaymentType Type, // CreditCard
  string CardNumber,
  string CardHolder,
  string CardType,
  string? PaymentProvider = null,
  string? PaymentChannel = null,
  string? TransactionId = null,
  DateTime? PaidAt = null
) : PaymentInfoResponse(Type, PaymentProvider, PaymentChannel, TransactionId, PaidAt);

public record BankTransferPaymentInfoResponse(
  PaymentType Type, // BankTransfer
  string BankName,
  string AccountNumber,
  string? PaymentProvider = null,
  string? PaymentChannel = null,
  string? TransactionId = null,
  DateTime? PaidAt = null
) : PaymentInfoResponse(Type, PaymentProvider, PaymentChannel, TransactionId, PaidAt);

public sealed class PaymentInfoResponseJsonConverter : JsonConverter<PaymentInfoResponse>
{
  public override PaymentInfoResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    throw new NotSupportedException("Deserialization is not supported for PaymentInfoResponse.");
  }

  public override void Write(Utf8JsonWriter writer, PaymentInfoResponse value, JsonSerializerOptions options)
  {
    switch (value)
    {
      case CashPaymentInfoResponse cash:
        JsonSerializer.Serialize(writer, cash, options);
        break;
      case CreditCardPaymentInfoResponse creditCard:
        JsonSerializer.Serialize(writer, creditCard, options);
        break;
      case BankTransferPaymentInfoResponse bankTransfer:
        JsonSerializer.Serialize(writer, bankTransfer, options);
        break;
      default:
        throw new JsonException($"Unsupported payment info type: {value.GetType().Name}");
    }
  }
}

public record OrderItemResponse(
  Guid ProductId,
  string ProductName,
  int Quantity,
  string Sku,
  decimal SellingPrice,
  decimal PurchasePrice,
  string? ThumbnailUrl,
  List<OrderAttributeResponse> Attributes);

public record OrderResponse(
  Guid Id,
  Guid UserId,
  string? DiscountCode,
  int? PointsUsed,
  decimal SubTotal,
  decimal ShippingFee,
  decimal DiscountAmount,
  decimal GrandAmount,
  PaymentStatusEnum PaymentStatus,
  OrderStatusEnum Status,
  ShippingInfoResponse ShippingInfo,
  PaymentInfoResponse PaymentInfo,
  string? Note,
  List<OrderItemResponse> Items,
  List<OrderHistoryResponse> Histories,
  List<InventorysAllocationResponse> InventoryAllocations);

/// <summary>
/// Dùng để trả về dữ liệu đơn hàng trong trường hợp chỉ cần thông tin cơ bản của sản phẩm (ví dụ: tên sản phẩm, số lượng, giá bán) mà không cần chi tiết về giá mua, SKU, thuộc tính, v.v.
/// </summary>
/// <param name="ProductName"></param>
/// <param name="Quantity"></param>
/// <param name="SellingPrice"></param>
/// <param name="ThumbnailUrl"></param>
public record OrderItemQueryResponse(
  Guid ProductId,
  string ProductName,
  int Quantity,
  decimal SellingPrice,
  string? ThumbnailUrl);
public record ListOrdersResponse(
  Guid Id,
  decimal GrandAmount,
  OrderStatusEnum Status,
  DateTime CreatedAt,
  List<OrderItemQueryResponse> Items);