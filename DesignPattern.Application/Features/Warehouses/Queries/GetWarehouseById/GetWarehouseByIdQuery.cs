using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.Warehouses.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.Warehouses.Queries.GetWarehouseById;

public record GetWarehouseByIdQuery(Guid Id) : IRequest<Result<WarehouseResponse>>, IRequiresUserContext
{
  public Guid UserId { get; set; }
}
