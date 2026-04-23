using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Users.Queries.GetAddresses;

public record GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, Result<List<AddressResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetAddressesQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<List<AddressResponse>>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
  {
    var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;

    var addresses = user.Addresses.Select(a => new AddressResponse(
      Id: a.Id,
      ReceiverName: a.ReceiverName.Value,
      PhoneNumber: a.ReceiverPhone.Value,
      Country: a.Country,
      Province: a.Province,
      District: a.District,
      Ward: a.Ward,
      Street: a.Street,
      ProvinceCode: a.ProvinceCode,
      DistrictCode: a.DistrictCode,
      WardCode: a.WardCode
    )).ToList();

    return addresses;
  }
}