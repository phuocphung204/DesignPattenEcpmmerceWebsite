using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Enums;
using DesignPattern.Application.Abstractions;

namespace DesignPattern.Application.Features.Orders.Commands.ConfirmOrder;

public record ConfirmOrderCommand(Guid OrderId)
  : IRequest<Result<OrderResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }

  public UserRole[] Roles => new[] { UserRole.Manager, UserRole.Admin };
}