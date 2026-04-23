using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.Order;
using DesignPattern.Domain.ValueObjects.Payment;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class CreateOrderValidationContext
{
  private CreateOrderValidationContext(CreateOrderCommand request)
  {
    Request = request;
  }

  public CreateOrderCommand Request { get; }
  public CreateOrderDto Dto => Request.dto;
  public Guid UserId => Request.UserId;

  public Quantity PointsToRedeem { get; set; } = Quantity.Zero;
  public Code? CodeValue { get; set; }
  public Note? NoteValue { get; set; }
  public ShippingInfo ShippingInfo { get; set; } = default!;
  public DiscountCode? DiscountCode { get; set; }
  public List<OrderItem> OrderItems { get; set; } = new();
  public PaymentInfo PaymentInfo { get; set; } = default!;

  public static CreateOrderValidationContext Create(CreateOrderCommand request) => new(request);
}
