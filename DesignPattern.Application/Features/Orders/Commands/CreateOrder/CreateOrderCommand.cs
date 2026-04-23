using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Abstractions;
namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder;

public record ShippingInfoDto(
  ShippingType Type, // Standard, Express
  Guid shippingAddressId
  );

public record PaymentInfoDto(
  PaymentType Type, // Cash, CreditCard, BankTransfer
  string? CardNumber,
  string? CardHolder,
  string? CardType,
  string? BankName,
  string? AccountNumber);

public record OrderItemDto(
  Guid ProductId,
  int RequiredQuantity
  );
public record CreateOrderDto(
  string? DiscountCode,
  int PointsUsed,
  ShippingInfoDto ShippingInfo,
  PaymentInfoDto PaymentInfo,
  string? Note,
  List<OrderItemDto> Items);
public record CreateOrderCommand(
    CreateOrderDto dto
) : IRequest<Result<OrderResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
};