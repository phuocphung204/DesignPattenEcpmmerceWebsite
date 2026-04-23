using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Users.Queries.GetAddresses;

public record GetAddressesQuery() : IRequest<Result<List<AddressResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
};