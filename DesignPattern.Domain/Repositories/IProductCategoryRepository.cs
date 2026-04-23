using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.ProductCategory;

namespace DesignPattern.Domain.Repositories;

public interface IProductCategoryRepository : IRepository<ProductCategory>, IForTest<ProductCategory>
{
  Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken);

  /// <summary>
  /// Lấy danh sách danh mục sản phẩm phân trang, hỗ trợ tìm kiếm theo tên
  /// (partial match, case-insensitive). Kết quả bao gồm thông tin Slug (từ Seo)
  /// và ParentCategory (Id + Name).
  /// </summary>
  Task<PagedResult<ProductCategoryReadModel>> SearchCategoriesPagedAsync(
    string? nameSearchTerm = null,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);
}

/// <summary>
/// Read model cho danh mục sản phẩm - dùng cho listing/search (CQRS read-side).
/// Không sử dụng Value Objects, chỉ chứa dữ liệu thô.
/// </summary>
public sealed record ProductCategoryReadModel
{
  public Guid Id { get; init; }
  public string Name { get; init; } = string.Empty;
  public string? Slug { get; init; }
  public int Level { get; init; }
  public Guid? ParentCategoryId { get; init; }
  public string? ParentCategoryName { get; init; }
}