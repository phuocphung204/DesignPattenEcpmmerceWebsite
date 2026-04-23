using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusDTO(
  OrderStatusEnum NewStatus,
  string? Note
);
public record UpdateOrderStatusCommand(Guid OrderId, UpdateOrderStatusDTO dto) : IRequest<Result<OrderResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}