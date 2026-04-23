using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;

namespace DesignPattern.Domain.Repositories;

public interface IProductRepository : IRepository<Product>, IForTest<Product>
{
  Task<List<Product>> GetProductsByVariantGroupIdAsync(string variantGroupId, CancellationToken cancellationToken = default);
  Task<PagedResult<ProductReadModel>> SearchProductsPagedAsync(
      ProductSearchCriteria criteria,
      int pageIndex = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default);
  Task<List<Product>> GetTopSellingProductsAsync(int topN, CancellationToken cancellationToken = default);
}

public sealed record ProductReadModel
{
  public Guid Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public string Image { get; init; } = string.Empty;
  public decimal SellingPrice { get; init; }
  public int SoldQuantity { get; init; } = 0;
  public decimal Rating { get; init; } = 0.0m;
  public string ShortDescription { get; init; } = string.Empty;
}

public enum Field { Price, Rating, SoldQuantity, CreatedAt };
public enum Direction { Asc, Desc };
public sealed record SortOption
{
  public Field? sortBy { get; init; } = null;
  public Direction? sortDirection { get; init; } = null;
}
public sealed record ProductSearchCriteria
{
  public SortOption? SortOption { get; init; } = null;
  public int PageIndex { get; init; } = 1;
  public int PageSize { get; init; } = 10;
  public string? Name { get; init; } = null;
  public Guid? BrandId { get; init; } = null;
  public Guid? CategoryId { get; init; } = null;
  public string? VariantGroupId { get; init; } = null;
  public string? Sku { get; init; } = null;
  public decimal? MinRating { get; init; } = null;
  public decimal? MaxRating { get; init; } = null;
  public decimal? MinPrice { get; init; } = null;
  public decimal? MaxPrice { get; init; } = null;
}