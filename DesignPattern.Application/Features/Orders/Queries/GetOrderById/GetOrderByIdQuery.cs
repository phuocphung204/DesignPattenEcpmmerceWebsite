using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}