using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.CreateUserAddress;

public record CreateUserAddressDTO(
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

public record CreateUserAddressCommand(CreateUserAddressDTO dto) : IRequest<Result<AddressResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}
