using DesignPattern.Application.Features.Users.Queries.GetUserProfile;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.LoginCommand;

public sealed record LoginCommand(
  string Email,
  string Password
  ) : IRequest<Result<LoginResponse>>
{ }

public sealed class LoginResponse
{
  public string Token { get; set; } = string.Empty; // Lưu userId, role
  public string UserId { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string FullName { get; set; } = string.Empty;
  public string AvatarLink { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public int LoyaltyPoints { get; set; }
  public Guid DefaultShippingAddressId { get; set; }
  public List<AddressResponse> Address { get; set; } = new();
  public List<LinkedAccountResponse> LinkedAccounts { get; set; } = new();
}
