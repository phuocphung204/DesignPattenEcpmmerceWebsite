using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.CreateUserAddress;

public sealed class CreateUserAddressCommandHandler
  : IRequestHandler<CreateUserAddressCommand, Result<AddressResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public CreateUserAddressCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<AddressResponse>> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var user = await _unitOfWork.UserRepository.GetAddressesByUserIdAsync(request.UserId, cancellationToken);

    if (user is null)
      return UserErrors.UserNotFound;

    var addressResult = Address.Create(
      receiverName: dto.ReceiverName,
      phoneNumber: dto.PhoneNumber,
      country: dto.Country,
      province: dto.Province,
      district: dto.District,
      ward: dto.Ward,
      street: dto.Street,
      provinceCode: dto.ProvinceCode,
      districtCode: dto.DistrictCode,
      wardCode: dto.WardCode);

    if (addressResult.IsFailure)
      return addressResult.Error;

    var addAddressResult = user.AddAddress(addressResult.Value);
    if (addAddressResult.IsFailure)
      return addAddressResult.Error;

    _unitOfWork.UserRepository.UpdateAddresses(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return AddressResponse.FromDomain(addressResult.Value);
  }

}
