namespace DesignPattern.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserDTO(
  string FullName,
  string Email,
  // string PhoneNumber,
  string Password,
  AddressDTO Address
);

public record AddressDTO(
  string ReceiverName,
  string PhoneNumber,
  string Country,
  string Province,
  string District,
  string Ward,
  string Street,
  string ProvinceCode,
  string DistrictCode,
  string WardCode
);
