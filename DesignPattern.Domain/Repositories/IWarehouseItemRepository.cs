using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;

namespace DesignPattern.Domain.Repositories;

public interface IWarehouseItemRepository : IRepository<WarehouseItem>, IForTest<WarehouseItem>
{
  Task<List<WarehouseItem>> GetListWarehouseItemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
  Task<List<WarehouseItem>> SearchAsync(WarehouseItemSearchCriteria criteria, CancellationToken cancellationToken = default);
  Task<PagedResult<WarehouseItem>> GetPagedAsync(
    WarehouseItemSearchCriteria criteria,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);
}
public record WarehouseItemReadModel
{
  public Guid Id { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public string Sku { get; init; } = string.Empty;
  public string? VariantGroupId { get; init; }
  public int Quantity { get; init; }
}

public sealed record WarehouseItemSearchCriteria
{
  public Guid? WarehouseId { get; init; }
  public string? ProductName { get; init; }
  public string? Sku { get; init; }
  public string? VariantGroupId { get; init; }
  public Guid? ProductId { get; init; }
}