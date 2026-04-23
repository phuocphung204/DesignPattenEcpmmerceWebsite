using DomainCategory = DesignPattern.Domain.Entities.ProductCategory.ProductCategory;

namespace DesignPattern.Application.Features.Seos.Shared;

public sealed class ProductCategoryResponse
{
  public required string Id { get; set; }
  public string? Name { get; set; }
  public int? Level { get; set; }
  public string? ParentCategoryId { get; set; }
  public string? MetaTitle { get; set; }
  public string? Slug { get; set; }
  public string? MetaDescription { get; set; }

  public ProductCategoryResponse() { }
  public static ProductCategoryResponse FromDomain(DomainCategory category)
  {
    ArgumentNullException.ThrowIfNull(category);

    return new ProductCategoryResponse
    {
      Id = category.Id.ToString(),
      Name = category.Name?.Value,
      Level = category.Level?.Value,
      ParentCategoryId = category.ParentCategory?.Id.ToString(),
      MetaTitle = category.Seo?.Title?.Value,
      Slug = category.Seo?.Slug?.Value,
      MetaDescription = category.Seo?.Description?.Value
    };
  }
}
