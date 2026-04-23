using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler
: IRequestHandler<UpdateUserProfileCommand, Result<UpdateUserProfileResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<UpdateUserProfileResponse>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    // 1. Kiểm tra user tồn tại

    var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;

    // 2. Kiểm tra trùng số điện thoại (nếu có cập nhật)
    // if (dto.PhoneNumber is not null)
    // {
    //   var isPhoneConflict = await _unitOfWork.UserRepository
    //     .IsPhoneExistExcludingUserAsync(dto.PhoneNumber, request.UserId, cancellationToken);

    //   if (isPhoneConflict)
    //     return UserErrors.PhoneAlreadyExists;
    // }

    // 3. Gọi domain method để cập nhật thông tin profile và set default address (nếu có)
    var updateResult = user.UpdateProfile(
      fullName: dto.FullName,
      // phoneNumber: dto.PhoneNumber,
      email: dto.Email,
      avatarLink: dto.AvatarLink);

    if (updateResult.IsFailure)
      return updateResult.Error;

    if (dto.DefaultAddressId is Guid defaultAddressId)
    {
      var settingDefaultAddressResult = user.SetDefaultAddress(defaultAddressId);
      if (settingDefaultAddressResult.IsFailure)
        return settingDefaultAddressResult.Error;
    }

    // 4. Persist
    _unitOfWork.UserRepository.Update(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new UpdateUserProfileResponse(
      user.Id,
      user.Email.Value,
      user.FullName.Value,
      // user.PhoneNumber.Value,
      user.AvatarLink,
      user.DefaultAddressId.GetValueOrDefault()
    );
  }
}
