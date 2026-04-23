using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects.Payment;
using DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;

namespace DesignPattern.Application.Features.Orders.Shared;

public static class OrderResponseMapper
{
  public static Result<OrderResponse> MapToOrderResponse(Order order)
  {
    PaymentInfoResponse paymentInfoResponse;
    switch (order.PaymentInfo)
    {
      case Cash:
        paymentInfoResponse = new CashPaymentInfoResponse(
          PaymentType.Cash,
          null,
          null,
          null,
          order.PaymentInfo.PaidAt
        );
        break;
      case CreditCard creditCardData:
        paymentInfoResponse = new CreditCardPaymentInfoResponse(
          PaymentType.CreditCard,
          creditCardData.CardNumber.Value,
          creditCardData.CardHolder.Value,
          creditCardData.CardType.Value,
          creditCardData.PaymentProvider,
          creditCardData.PaymentChannel,
          creditCardData.TransactionId,
          creditCardData.PaidAt
        );
        break;
      case BankTransfer bankTransferData:
        paymentInfoResponse = new BankTransferPaymentInfoResponse(
          PaymentType.BankTransfer,
          bankTransferData.BankName.Value,
          bankTransferData.AccountNumber.Value,
          bankTransferData.PaymentProvider,
          bankTransferData.PaymentChannel,
          bankTransferData.TransactionId,
          bankTransferData.PaidAt
        );
        break;
      default:
        return OrderErrors.InvalidOrderPaymentMethod;
    }

    var shippingInfoResponse = new ShippingInfoResponse(
      order.ShippingInfo.Type,
      new ShippingAddressResponse(
        order.ShippingInfo.Address.ReceiverName.Value,
        order.ShippingInfo.Address.ReceiverPhone.Value,
        order.ShippingInfo.Address.Country,
        order.ShippingInfo.Address.Province,
        order.ShippingInfo.Address.District,
        order.ShippingInfo.Address.Ward,
        order.ShippingInfo.Address.Street,
        order.ShippingInfo.Address.ProvinceCode,
        order.ShippingInfo.Address.DistrictCode,
        order.ShippingInfo.Address.WardCode
      )
    );

    var itemResponses = order.Items.Select(item =>
      new OrderItemResponse(
        item.ProductId,
        item.ProductName.Value,
        item.Quantity.Value,
        item.Sku.Value,
        item.SellingPrice.Amount,
        item.PurchasePrice.Amount,
        item.ThumbnailUrl,
        item.Attributes
          .Select(attr => new OrderAttributeResponse(attr.Name, attr.Value))
          .ToList()
      )
    ).ToList();

    var historyResponses = order.Histories.Select(history =>
      new OrderHistoryResponse(
        history.Status,
        history.StatusChangedDate,
        history.ChangedBy,
        history.ChangedByRole,
        history.Note
      )
    ).ToList();

    var inventoryAllocationResponses = order.InventoryAllocations.Select(allocation =>
      new InventorysAllocationResponse
      {
        WarehouseId = allocation.WarehouseId,
        ProductId = allocation.ProductId,
        AllocatedQuantity = allocation.Allocated.Value
      }
    ).ToList();

    var orderResponse = new OrderResponse(
      Id: order.Id,
      UserId: order.UserId,
      DiscountCode: order.DiscountCode?.Value,
      PointsUsed: order.PointsUsed?.Value,
      SubTotal: order.SubTotal.Amount,
      ShippingFee: order.ShippingFee.Amount,
      DiscountAmount: order.DiscountAmount.Amount,
      GrandAmount: order.GrandAmount.Amount,
      PaymentStatus: order.PaymentStatus,
      Status: order.Status,
      ShippingInfo: shippingInfoResponse,
      PaymentInfo: paymentInfoResponse,
      Note: order.Note?.Value,
      Items: itemResponses,
      Histories: historyResponses,
      InventoryAllocations: inventoryAllocationResponses
    );

    return orderResponse;
  }
}