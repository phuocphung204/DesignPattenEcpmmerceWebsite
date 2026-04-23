using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.User;

namespace DesignPattern.Domain.Entities.Users;

public class Address : BaseEntity
{
  public Name ReceiverName { get; private set; }
  public PhoneNumber ReceiverPhone { get; private set; }
  public string Country { get; private set; }
  public string Province { get; private set; }
  public string District { get; private set; }
  public string Ward { get; private set; }
  public string Street { get; private set; }
  public string ProvinceCode { get; private set; }
  public string DistrictCode { get; private set; }
  public string WardCode { get; private set; }

  private Address(
      Name receiverName,
      PhoneNumber phoneNumber,
      string country,
      string province,
      string district,
      string ward,
      string street,
      string provinceCode,
      string districtCode,
      string wardCode)
  {
    ReceiverName = receiverName;
    ReceiverPhone = phoneNumber;
    Country = country;
    Province = province;
    District = district;
    Ward = ward;
    Street = street;
    ProvinceCode = provinceCode;
    DistrictCode = districtCode;
    WardCode = wardCode;
  }

  // This constructor is for rehydration only, not for creating new addresses
  private Address(
      Guid id,
      DateTime createdAt,
      DateTime? updatedAt,
      Name receiverName,
      PhoneNumber phoneNumber,
      string country,
      string province,
      string district,
      string ward,
      string street,
      string provinceCode,
      string districtCode,
      string wardCode) : this(
        receiverName,
        phoneNumber,
        country,
        province,
        district,
        ward,
        street,
        provinceCode,
        districtCode,
        wardCode)
  {
    Id = id;
    CreatedAt = createdAt;
    UpdatedAt = updatedAt;
  }

  public static Result<Address> Create(
      string receiverName,
      string phoneNumber,
      string country,
      string province,
      string district,
      string ward,
      string street,
      string provinceCode,
      string districtCode,
      string wardCode
      )
  {

    Result<Name> nameResult = Name.Create(receiverName);
    if (nameResult.IsFailure) return nameResult.Error;

    Result<PhoneNumber> phoneResult = PhoneNumber.Create(phoneNumber);
    if (phoneResult.IsFailure) return phoneResult.Error;

    Address address = new Address(
        nameResult.Value,
        phoneResult.Value,
        country,
        province,
        district,
        ward,
        street,
        provinceCode,
        districtCode,
        wardCode
    );

    return address;
  }

  public Result Update(
    string? receiverName,
    string? phoneNumber,
    string? country,
    string? province,
    string? district,
    string? ward,
    string? street,
    string? provinceCode,
    string? districtCode,
    string? wardCode)
  {
    if (receiverName != null)
    {
      Result<Name> nameResult = Name.Create(receiverName);
      if (nameResult.IsFailure)
        return nameResult.Error;
      ReceiverName = nameResult.Value;
    }

    if (phoneNumber != null)
    {
      Result<PhoneNumber> phoneResult = PhoneNumber.Create(phoneNumber);
      if (phoneResult.IsFailure)
        return phoneResult.Error;
      ReceiverPhone = phoneResult.Value;
    }

    if (country != null) Country = country;
    if (province != null) Province = province;
    if (district != null) District = district;
    if (ward != null) Ward = ward;
    if (street != null) Street = street;
    if (provinceCode != null) ProvinceCode = provinceCode;
    if (districtCode != null) DistrictCode = districtCode;
    if (wardCode != null) WardCode = wardCode;

    return true;
  }
}
