using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Application.Abstractions;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetItemById;

public record GetItemByIdQuery(Guid Id) : IRequest<Result<WarehouseItemResponse?>>, IRequiresUserContext
{
  public Guid UserId { get; set; }
}