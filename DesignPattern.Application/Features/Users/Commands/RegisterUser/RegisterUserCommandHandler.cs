using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;
using MediatR;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
{
  private readonly IUnitOfWork _unitOfWork;

  public RegisterUserCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;

    var isEmailExists = await _unitOfWork.UserRepository.IsEmailExistAsync(dto.Email, cancellationToken);
    if (isEmailExists)
    {
      return Error.Conflict(
        "User.EmailAlreadyExists",
        "User already exists with the provided email.");
    }

    // var isPhoneExists = await _unitOfWork.UserRepository.IsPhoneExistAsync(dto.PhoneNumber, cancellationToken);
    // if (isPhoneExists)
    // {
    //   return Error.Conflict(
    //     "User.PhoneAlreadyExists",
    //     "User already exists with the provided phone number.");
    // }

    var address = dto.Address;

    var addressResult = Address.Create(
      receiverName: address.ReceiverName,
      phoneNumber: address.PhoneNumber,
      country: address.Country,
      province: address.Province,
      district: address.District,
      ward: address.Ward,
      street: address.Street,
      provinceCode: address.ProvinceCode,
      districtCode: address.DistrictCode,
      wardCode: address.WardCode
    );

    if (addressResult.IsFailure)
    {
      return Result.Failure(addressResult.Error);
    }

    var userResult = User.Create(
      fullName: dto.FullName,
      email: dto.Email,
      password: dto.Password,
      role: UserRole.Customer,
      address: addressResult.Value);

    if (userResult.IsFailure)
    {
      return Result.Failure(userResult.Error);
    }

    _unitOfWork.UserRepository.Create(userResult.Value);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // Tạo cart mới cho user ngay sau khi đăng ký và có user trên csdl
    var newCart = Cart.Create(userResult.Value.Id);
    _unitOfWork.CartRepository.Create(newCart);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }

}
