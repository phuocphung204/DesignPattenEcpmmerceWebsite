using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Warehouses.Commands.DeleteWarehouse;

public record DeleteWarehouseCommand(Guid WarehouseId) : IRequest<Result>, IRequiresUserContext, IAuthorizeableRequest
{
  public Guid UserId { get; set; }
  public UserRole[] Roles => new[] { UserRole.Manager };
}