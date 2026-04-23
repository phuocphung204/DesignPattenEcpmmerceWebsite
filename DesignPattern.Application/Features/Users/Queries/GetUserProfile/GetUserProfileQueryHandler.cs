using MediatR;
using DesignPattern.Domain.Enums;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Users.Shared;

namespace DesignPattern.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetUserProfileQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<UserResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
  {
    var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;

    var linkedAccounts = user.LinkedAccounts.Select(la => new LinkedAccountResponse(
      Provider: la.Provider.Value,
      ProviderId: la.ProviderId.Value
    )).ToList();

    return new UserResponse(
      Id: user.Id,
      FullName: user.FullName.Value,
      Email: user.Email.Value,
      AvatarUrl: user.AvatarLink,
      LoyaltyPoints: user.LoyaltyPoints.Value,
      LinkedAccounts: linkedAccounts
    );

  }
}