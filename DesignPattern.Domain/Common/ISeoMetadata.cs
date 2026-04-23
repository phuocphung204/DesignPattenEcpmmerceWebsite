using DesignPattern.Domain.ValueObjects.Seo;

namespace DesignPattern.Domain.Common;

public interface ISeoMetadata
{
  Slug? Slug { get; }
  MetaTitle? MetaTitle { get; }
  MetaDescription? MetaDescription { get; }
  string? MetaKeywords { get; }
}