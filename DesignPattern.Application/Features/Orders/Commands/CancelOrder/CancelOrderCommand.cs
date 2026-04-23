using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommandDto
(
  string Note // Lý do hủy đơn hàng
);
public record CancelOrderCommand(Guid OrderId, CancelOrderCommandDto dto)
  : IRequest<Result<OrderResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }

  public UserRole[] Roles => new[] { UserRole.Customer, UserRole.Manager };
}
