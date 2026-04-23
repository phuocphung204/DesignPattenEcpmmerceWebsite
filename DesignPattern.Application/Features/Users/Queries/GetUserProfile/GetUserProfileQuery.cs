using MediatR;
using DesignPattern.Domain.Enums;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Users.Shared;

namespace DesignPattern.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQuery : IRequest<Result<UserResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}

