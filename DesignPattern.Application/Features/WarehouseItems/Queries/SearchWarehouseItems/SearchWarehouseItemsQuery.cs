using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using MediatR;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.SearchWarehouseItems;

public sealed class SearchWarehouseItemsQuery : IRequest<Result<List<WarehouseItemResponse>>>
{
  public Guid? WarehouseId { get; init; }
  public string? ProductName { get; init; }
  public string? Sku { get; init; }
  public string? VariantGroupId { get; init; }
  public Guid? ProductId { get; init; }
}
