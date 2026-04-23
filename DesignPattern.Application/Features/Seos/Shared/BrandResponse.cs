using DesignPattern.Domain.Entities;

namespace DesignPattern.Application.Features.Seos.Shared;

public sealed class BrandResponse
{
  public string Id { get; set; }
  public string Name { get; set; }
  public int? Level { get; set; }
  public string? ParentBrandId { get; set; }
  public string? MetaTitle { get; set; }
  public string? Slug { get; set; }
  public string? MetaDescription { get; set; }

  public BrandResponse() { }

  public static BrandResponse FromDomain(Brand brand)
  {
    return new BrandResponse
    {
      Id = brand.Id.ToString(),
      Name = brand.Name?.Value!,
      Level = brand.Level?.Value,
      ParentBrandId = brand.ParentBrand?.Id.ToString(),
      MetaTitle = brand.Seo?.Title?.Value,
      Slug = brand.Seo?.Slug?.Value,
      MetaDescription = brand.Seo?.Description?.Value

    };
  }
}
