using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Helpers;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Entities.ProductCategory;

public class ProductCategory : BaseEntity
{
  public Name Name { get; protected set; }
  public Level Level { get; protected set; }
  public ProductCategory? ParentCategory { get; protected set; }
  public Seo Seo { get; protected set; } = new();

#pragma warning disable CS8618
  private ProductCategory() { }
#pragma warning restore CS8618

  public static Result<ProductCategory> Create(string name, int level, string? metaTitle, string? slug, string? metaDescription, ProductCategory? parentCategory = null)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    var levelResult = Level.Create(level);
    if (levelResult.IsFailure) return levelResult.Error;

    var seoResult = Seo.Create(metaTitle, metaDescription, slug);
    if (seoResult.IsFailure) return seoResult.Error;

    var category = new ProductCategory
    {
      Name = nameResult.Value,
      Level = levelResult.Value,
      Seo = seoResult.Value,
      ParentCategory = parentCategory
    };

    return category;
  }

  public Result Update(string? name, string? metaTitle, string? metaDescription, string? slug)
  {
    if (name != null)
    {
      var nameResult = Name.Create(name);
      if (nameResult.IsFailure) return nameResult.Error;
      Name = nameResult.Value;
    }

    var seoResult = Seo.Update(metaTitle, metaDescription, slug);
    if (seoResult.IsFailure) return seoResult.Error;

    UpdatedAt = DateTime.UtcNow;
    return true;
  }

  public Result ChangeParentCategory(ProductCategory? parentCategory)
  {
    if (parentCategory is null)
    {
      ParentCategory = null;
      Level = Level.Create(0).Value;
      return true;
    }
    if (parentCategory.Id == Id) return Error.Conflict(
      "ProductCategory.CannotChangeParentCategory",
      "Parent category cannot be the same as the current category.");

    var levelValue = parentCategory.Level.Value + 1;
    var levelResult = Level.Create(levelValue);
    if (levelResult.IsFailure) return levelResult.Error;

    Level = levelResult.Value;
    ParentCategory = parentCategory;

    return true;
  }
  public override string ToString()
  {
    return Name.Value;
  }
}
