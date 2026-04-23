using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserProfile;

public record UpdateUserProfileDTO(
  string? FullName = null,
  // string? PhoneNumber = null,
  string? Email = null,
  string? AvatarLink = null,
  Guid? DefaultAddressId = null
);

public record UpdateUserProfileResponse(
  Guid Id,
  string Email,
  string FullName,
  // string PhoneNumber,
  string AvatarLink,
  Guid DefaultAddressId);

public record UpdateUserProfileCommand(UpdateUserProfileDTO dto) : IRequest<Result<UpdateUserProfileResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}
