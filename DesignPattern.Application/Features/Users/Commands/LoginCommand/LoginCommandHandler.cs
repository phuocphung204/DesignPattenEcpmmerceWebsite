using DesignPattern.Application.Features.Users.Queries.GetUserProfile;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.ValueObjects.User;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{

  private readonly IUnitOfWork _unitOfWork;

  public LoginCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var user = await _unitOfWork.UserRepository.GetAuthInfoByEmailAsync(request.Email, cancellationToken);
    if (user is null)
      return LoginErrors.EmailUnregistered;

    if (user.Status == UserStatus.InActive)
      return LoginErrors.UserInActive;

    if (user.Status == UserStatus.Banned)
      return LoginErrors.UserBanned;

    var isPasswordValid = string.Equals(PasswordHash.HashSHA256(request.Password), user.PasswordHash.Value);
    if (!isPasswordValid)
      return LoginErrors.InvalidCredentials;

    return new LoginResponse
    {
      UserId = user.Id.ToString(),
      Email = user.Email.Value,
      FullName = user.FullName.Value,
      AvatarLink = user.AvatarLink ?? "",
      Role = user.Role.ToString(),
      LoyaltyPoints = user.LoyaltyPoints.Value,
      DefaultShippingAddressId = user.DefaultAddressId ?? Guid.Empty,
      Address = user.Addresses.Select(a => new AddressResponse(
        a.Id,
        a.ReceiverName.Value,
        a.ReceiverPhone.Value,
        a.Country,
        a.Province,
        a.District,
        a.Ward,
        a.Street,
        a.ProvinceCode,
        a.DistrictCode,
        a.WardCode
      )).ToList(),
      LinkedAccounts = user.LinkedAccounts.Select(la => new LinkedAccountResponse(
        la.Provider.Value,
        la.ProviderId.Value
      )).ToList()
    };
  }
}
