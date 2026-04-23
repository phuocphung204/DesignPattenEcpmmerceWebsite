namespace DesignPattern.Application.Features.Seos.Shared;

public sealed record BrandListItemResponse
{
  public required string Id { get; init; }
  public required string Name { get; init; }
  public string? Slug { get; init; }
  public int Level { get; init; }
  public ParentBrandInfo? ParentBrand { get; init; }
}

public sealed record ParentBrandInfo
{
  public required string Id { get; init; }
  public string? Name { get; init; }
}
