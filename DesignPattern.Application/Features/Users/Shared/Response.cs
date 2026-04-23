using DesignPattern.Domain.Entities.Users;

namespace DesignPattern.Application.Features.Users.Shared;

public sealed record AddressResponse(
  Guid Id,
  string ReceiverName,
  string PhoneNumber,
  string Country,
  string Province,
  string District,
  string Ward,
  string Street,
  string ProvinceCode,
  string DistrictCode,
  string WardCode)
{
  public static AddressResponse FromDomain(Address address)
  {
    ArgumentNullException.ThrowIfNull(address);

    return new AddressResponse(
      address.Id,
      address.ReceiverName.Value,
      address.ReceiverPhone.Value,
      address.Country,
      address.Province,
      address.District,
      address.Ward,
      address.Street,
      address.ProvinceCode,
      address.DistrictCode,
      address.WardCode);
  }
}
public record LinkedAccountResponse(
  string Provider,
  string ProviderId
);
public record UserResponse(
  Guid Id,
  string FullName,
  string Email,
  string AvatarUrl,
  int LoyaltyPoints,
  List<LinkedAccountResponse> LinkedAccounts
);