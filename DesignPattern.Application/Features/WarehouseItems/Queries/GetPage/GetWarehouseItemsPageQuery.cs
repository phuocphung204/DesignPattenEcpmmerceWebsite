using DesignPattern.Application.Abstractions;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using MediatR;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetPage;

public sealed record GetWarehouseItemsPageQuery : IRequest<PagedResult<WarehouseItemsReadModel>>, IRequiresUserContext
{
  public Guid UserId { get; set; }
  public Guid? WarehouseId { get; init; }
  public string? ProductName { get; init; }
  public string? Sku { get; init; }
  public string? VariantGroupId { get; init; }
  public Guid? ProductId { get; init; }
  public int PageIndex { get; init; } = 1;
  public int PageSize { get; init; } = 10;
}
public record WarehouseItemsReadModel(
  Guid Id,
  string ProductName,
  string Sku,
  string? VariantGroupId,
  int Quantity
);
