using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Entities;

public class Brand : BaseEntity
{
  public Name Name { get; private set; }
  public Brand? ParentBrand { get; private set; }
  public Level Level { get; private set; }
  public Seo Seo { get; private set; } = new();

#pragma warning disable CS8618
  private Brand() { }
#pragma warning restore CS8618
  public static Result<Brand> Create(string name, int level, string? metaTitle, string? slug, string? metaDescription,
                                      Brand? parentBrand = null)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    var levelResult = Level.Create(level);
    if (levelResult.IsFailure) return levelResult.Error;

    var seoResult = Seo.Create(metaTitle, metaDescription, slug);
    if (seoResult.IsFailure) return seoResult.Error;

    var brand = new Brand
    {
      Name = nameResult.Value,
      Level = levelResult.Value,
      Seo = seoResult.Value,
      ParentBrand = parentBrand
    };

    return brand;
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

  public Result ChangeParentBrand(Brand? parentBrand)
  {
    if (parentBrand is null)
    {
      ParentBrand = null;
      Level = Level.Create(0).Value;
      return true;
    }

    if (parentBrand.Id == Id)
    {
      return Error.Conflict(
          "Brand.CannotChangeParentBrand",
          "Parent brand cannot be the same as the current brand.");
    }

    var levelValue = parentBrand.Level.Value + 1;
    var levelResult = Level.Create(levelValue);
    if (levelResult.IsFailure) return levelResult.Error;

    Level = levelResult.Value;
    ParentBrand = parentBrand;

    return true;
  }

  public override string ToString()
  {
    return $"[Name]: {Name.Value}\n[Level]: {Level.Value}\n[Seo]: {Seo?.ToString()}";
  }
}
