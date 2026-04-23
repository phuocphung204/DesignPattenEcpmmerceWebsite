
using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using MediatR;

namespace DesignPattern.Application.Features.Warehouses.Queries.GetPageWarehouse;

public record GetPageWarehouseQuery : IRequest<PagedResult<WarehouseReadModel>>, IRequiresUserContext
{
  public Guid UserId { get; set; }
  public string Name { get; init; }
  public string Address { get; init; }
  public int PageIndex { get; init; } = 1;
  public int PageSize { get; init; } = 10;
}
public record WarehouseReadModel(
  Guid Id,
  string Name,
  string Address
);