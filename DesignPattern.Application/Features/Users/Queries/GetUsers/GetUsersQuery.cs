using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;
using DomainUser = DesignPattern.Domain.Entities.Users.User;

namespace DesignPattern.Application.Features.Users.Queries.GetUsers;

public sealed record UserResponse(
  string Id,
  string FullName,
  string Email,
  // string PhoneNumber,
  string AvatarUrl,
  int LoyaltyPoints)
{
  // public UserResponse() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0) { }
  public static UserResponse FromDomain(DomainUser user)
  {
    ArgumentNullException.ThrowIfNull(user);

    // return new UserResponse(
    //     user.ID?.Value ?? string.Empty,
    //     user.FullName?.Value ?? string.Empty,
    //     user.Email?.Value ?? string.Empty,
    //     user.PhoneNumber?.Value ?? string.Empty,
    //     // user.AvatarUrl?.Value ?? string.Empty,
    //     user.LoyaltyPoints?.Value ?? 0);

    // return new UserResponse
    // {
    //   Id = user.ID?.Value ?? string.Empty,
    //   FullName = user.FullName?.Value ?? string.Empty,
    //   Email = user.Email?.Value ?? string.Empty,
    //   PhoneNumber = user.PhoneNumber?.Value ?? string.Empty,
    //   AvatarUrl = string.Empty,
    //   LoyaltyPoints = user.LoyaltyPoints?.Value ?? 0
    // };

    return new UserResponse(
      Id: user.Id.ToString() ?? string.Empty,
      FullName: user.FullName?.Value ?? string.Empty,
      Email: user.Email?.Value ?? string.Empty,
      // PhoneNumber: user.PhoneNumber?.Value ?? string.Empty,
      AvatarUrl: string.Empty,
      LoyaltyPoints: user.LoyaltyPoints?.Value ?? 0);
  }
}

public class GetUsersQuery : IRequest<Result<List<UserResponse>>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager, UserRole.Admin };
}

