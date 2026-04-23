using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;

namespace DesignPattern.Domain.Repositories;

public interface IWarehouseRepository : IRepository<Warehouse>, IForTest<Warehouse>
{
  Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
  Task<PagedResult<Warehouse>> GetPagedAsync(
    WarehouseCriteria criteria,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);
}

public sealed record WarehouseCriteria
{
  public string? Name { get; init; } = string.Empty;
  public string? Address { get; init; } = string.Empty;
}
