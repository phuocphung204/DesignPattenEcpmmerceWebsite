namespace DesignPattern.Application.Features.Seos.Shared;

/// <summary>
/// DTO trả về cho danh sách danh mục sản phẩm (listing/search).
/// Chứa các trường: Name, Slug, Level, ParentCategory (Id + Name).
/// </summary>
public sealed class CategoryListItemResponse
{
  public required string Id { get; set; }
  public string? Name { get; set; }
  public string? Slug { get; set; }
  public int Level { get; set; }
  public ParentCategoryInfo? ParentCategory { get; set; }
}

/// <summary>
/// Thông tin danh mục cha (chỉ gồm Id và Name).
/// </summary>
public sealed class ParentCategoryInfo
{
  public required string Id { get; set; }
  public string? Name { get; set; }
}
