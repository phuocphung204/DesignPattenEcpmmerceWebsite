using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Warehouses.Commands.UpdateWarehouse;

public record UpdateWarehouseDTO(
  string Name,
  string Address);
public record UpdateWarehouseCommand(
  Guid Id,
  UpdateWarehouseDTO dto) : IRequest<Result<WarehouseResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}