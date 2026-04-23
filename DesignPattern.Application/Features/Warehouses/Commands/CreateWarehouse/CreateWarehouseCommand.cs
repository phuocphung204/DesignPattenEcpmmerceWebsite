using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Warehouses.Commands.CreateWarehouse;

public record CreateWarehouseDTO(
  string Name,
  string Address);
public record CreateWarehouseCommand(
  CreateWarehouseDTO dto) : IRequest<Result<WarehouseResponse>>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}