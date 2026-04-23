using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects.User;

namespace DesignPattern.Domain.ValueObjects;

public class ShippingAddress
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

  private ShippingAddress(
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


  public static Result<ShippingAddress> Create(
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

    ShippingAddress address = new ShippingAddress(
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

  public static ShippingAddress Rehydrate(
      string receiverName,
      string phoneNumber,
      string country,
      string province,
      string district,
      string ward,
      string street,
      string provinceCode,
      string districtCode,
      string wardCode)
  {

    ShippingAddress address = new ShippingAddress(
      Name.Rehydrate(receiverName),
      PhoneNumber.Rehydrate(phoneNumber),
      country,
      province,
      district,
      ward,
      street,
      provinceCode,
      districtCode,
      wardCode);
    return address;
  }
}
