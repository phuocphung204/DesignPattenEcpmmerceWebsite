using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;

namespace DesignPattern.Domain.Repositories;

public interface IBrandRepository : IRepository<Brand>, IForTest<Brand>
{
  Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

  Task<PagedResult<BrandReadModel>> SearchBrandsPagedAsync(
      string? nameSearchTerm = null,
      int pageIndex = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default);
}

public sealed record BrandReadModel
{
  public Guid Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public string? Slug { get; init; }
  public int Level { get; init; }
  public Guid? ParentBrandId { get; init; }
  public string? ParentBrandName { get; init; }
}
