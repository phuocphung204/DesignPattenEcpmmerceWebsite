using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Order;
using DesignPattern.Domain.ValueObjects.Payment;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
  {
    var context = CreateOrderValidationContext.Create(request);
    var validationChain = BuildValidationChain();
    var validationResult = await validationChain.HandleAsync(context, cancellationToken, this);
    if (validationResult.IsFailure)
      return validationResult.Error;

    // 5. Validate and create Order
    var orderResult = Order.Create(
      request.UserId,
      context.OrderItems,
      context.ShippingInfo,
      context.PaymentInfo,
      context.CodeValue,
      context.PointsToRedeem,
      context.NoteValue
    );
    if (orderResult.IsFailure)
      return orderResult.Error;
    var order = orderResult.Value;

    // 6. If discount code is applied, calculate total with discount and update order totals; otherwise calculate total without discount and update order totals
    // 6.1 Calculate total with discount code if applicable
    if (order.DiscountCode is not null && context.DiscountCode is not null)
    {
      var subtotal = order.CalculateSubTotal();
      var applyDiscountResult = context.DiscountCode.CheckAppliableDiscount(subtotal);
      if (applyDiscountResult.IsFailure)
        return applyDiscountResult.Error;
      // Update order totals with discount 
      order.UpdateCalculationWithDiscount(applyDiscountResult.Value);
    }
    else // 6.2 If no discount code, calculate total without discount
      // Update order totals without discount 
      order.UpdateCalculationWithoutDiscount();

    _unitOfWork.OrderRepository.Create(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // Respond with OrderCommandResponse
    var orderResponse = OrderResponseMapper.MapToOrderResponse(order);
    if (orderResponse.IsFailure)
      return orderResponse.Error;

    return orderResponse.Value;
  }

  private static ICreateOrderValidationStep BuildValidationChain()
  {
    var valueObjectsStep = new ValueObjectsValidationStep();
    valueObjectsStep
      .SetNext(new ShippingInfoValidationStep())
      .SetNext(new DiscountCodeValidationStep())
      .SetNext(new OrderItemsValidationStep())
      .SetNext(new PaymentInfoValidationStep());

    return valueObjectsStep;
  }

  public async Task<Result<DiscountCode?>> ValidateAndGetDiscountCode(string? discountCode)
  {
    if (discountCode is not null)
    {
      var discountCodeEntity = await _unitOfWork.DiscountCodeRepository.GetByCodeAsync(discountCode);
      if (discountCodeEntity is null)
        return OrderErrors.InvalidDiscountCode;

      return discountCodeEntity;
    }

    return Result<DiscountCode?>.Success(null);
  }
  public async Task<Result<List<OrderItem>>> ValidateAvailabilityOfProductsAndCreateOrderItems(List<OrderItemDto> dtos, CancellationToken cancellationToken)
  {
    var orderItems = new List<OrderItem>();

    // Validate stock by aggregated quantity per product to avoid overselling when the same product appears multiple times.
    var requiredQuantitiesByProduct = dtos
      .GroupBy(dto => dto.ProductId)
      .ToDictionary(group => group.Key, group => group.Sum(item => item.RequiredQuantity));

    foreach (var requiredItem in requiredQuantitiesByProduct)
    {
      var warehouseItems = await _unitOfWork.WarehouseItemRepository
        .GetListWarehouseItemByProductIdAsync(requiredItem.Key, cancellationToken);

      if (warehouseItems.Count == 0)
        return WarehouseErrors.CheckWarehouseItemNotFound(requiredItem.Key);

      var stock = warehouseItems.Sum(item => item.GetStock().Value);
      if (stock < requiredItem.Value)
        return WarehouseErrors.InsufficientStock(requiredItem.Key, requiredItem.Value, stock);
    }

    foreach (var dto in dtos)
    {
      // Validate product existence and get product details for order item creation
      var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId, cancellationToken);
      if (product is null)
        return ProductErrors.NotFound(dto.ProductId);
      var orderItemResult = OrderItem.Create(
        dto.ProductId,
        product.Name,
        dto.RequiredQuantity,
        product.Sku,
        product.SellingPrice,
        product.PurchasePrice,
        product.Images.FirstOrDefault(),
        product.Attributes.Where(attr => attr.Type == ProductAttributeType.Appearance).Select(attr => new AttributeItem(attr.Name.Value, attr.Value.Value)).ToList()
      );
      if (orderItemResult.IsFailure)
        return orderItemResult.Error;
      orderItems.Add(orderItemResult.Value);
    }
    return orderItems;
  }
  public async Task<Result<ShippingInfo>> ValidateUserLoyaltyPointsAndCreateShippingInfo(Guid userId, ShippingInfoDto dto, Quantity pointsToRedeem)
  {
    var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
    if (user is null)
      return OrderErrors.InvalidUserId;
    // 1. Validate user and loyalty points if applicable
    if (user.CheckInsufficientLoyaltyPoints(pointsToRedeem))
      return OrderErrors.InsufficientLoyaltyPoints;
    // get shipping address from user's saved addresses
    var ShippingAddress = user.Addresses.FirstOrDefault(addr => addr.Id == dto.shippingAddressId);
    if (ShippingAddress is null)
      return OrderErrors.InvalidDefaultShippingAddressId;

    var shippingInfoResult = ShippingInfo.Create(dto.Type, ShippingAddress);
    if (shippingInfoResult.IsFailure)
      return shippingInfoResult.Error;
    return shippingInfoResult.Value;
  }
  public Result<PaymentInfo> ValidateAndCreatePaymentInfo(PaymentInfoDto dto)
  {
    PaymentData data;
    switch (dto.Type)
    {
      case PaymentType.Cash:
        data = new CashPaymentData();
        break;
      case PaymentType.CreditCard:
        data = new CreditCardPaymentData(
          dto.CardNumber!,
          dto.CardHolder!,
          dto.CardType!
        );
        break;
      case PaymentType.BankTransfer:
        data = new BankTransferPaymentData(
          dto.BankName!,
          dto.AccountNumber!
        );
        break;
      default:
        return OrderErrors.InvalidOrderPaymentMethod;
    }
    var paymentInfoResult = PaymentFactory.Create(data);
    if (paymentInfoResult.IsFailure)
      return paymentInfoResult.Error;

    return paymentInfoResult.Value;
  }
}
