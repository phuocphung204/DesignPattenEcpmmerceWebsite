using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Users.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using MediatR;

namespace DesignPattern.Application.Features.Users.Commands.UpdateUserAddress;

public record UpdateUserAddressDTO(
  string? ReceiverName = null,
  string? PhoneNumber = null,
  string? Country = null,
  string? Province = null,
  string? District = null,
  string? Ward = null,
  string? Street = null,
  string? ProvinceCode = null,
  string? DistrictCode = null,
  string? WardCode = null
);

public record UpdateUserAddressCommand(UpdateUserAddressDTO dto, Guid addressId)
  : IRequest<Result<AddressResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Customer };
}
