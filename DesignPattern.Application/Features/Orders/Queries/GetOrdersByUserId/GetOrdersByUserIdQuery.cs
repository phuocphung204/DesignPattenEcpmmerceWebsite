using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
namespace DesignPattern.Application.Features.Orders.Queries.GetOrdersByUserId;

public record GetOrdersByUserIdQuery() : IRequest<Result<List<ListOrdersResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
};