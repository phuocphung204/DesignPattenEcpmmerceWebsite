using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand, Result<AddressResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public UpdateUserAddressCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<AddressResponse>> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var user = await _unitOfWork.UserRepository.GetAddressesByUserIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;

    var address = user.Addresses.FirstOrDefault(a => a.Id == request.addressId);
    if (address is null)
    {
      return Error.Conflict(
        "Invalid.Address",
        "Address does not belong to user");
    }

    var updateResult = user.UpdateAddress(
      request.addressId,
      dto.ReceiverName,
      dto.PhoneNumber,
      dto.Country,
      dto.Province,
      dto.District,
      dto.Ward,
      dto.Street,
      dto.ProvinceCode,
      dto.DistrictCode,
      dto.WardCode);

    if (updateResult.IsFailure)
      return updateResult.Error;

    _unitOfWork.UserRepository.UpdateAddresses(user);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new AddressResponse(
      Id: address.Id,
      ReceiverName: address.ReceiverName.Value,
      PhoneNumber: address.ReceiverPhone.Value,
      Country: address.Country,
      Province: address.Province,
      District: address.District,
      Ward: address.Ward,
      Street: address.Street,
      ProvinceCode: address.ProvinceCode,
      DistrictCode: address.DistrictCode,
      WardCode: address.WardCode
    );
  }
}
