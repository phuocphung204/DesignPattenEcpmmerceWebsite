using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Orders.Queries.SearchListOrder;


public record SearchListOrderQuery(
  DateTime Start,
  DateTime End,
  string? IdSearch,
  int PageIndex = 1,
  int PageSize = 10
  ) : IRequest<Result<PagedResult<ListOrdersResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager, UserRole.Admin };
};
