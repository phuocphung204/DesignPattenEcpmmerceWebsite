using DesignPattern.Domain.Helpers;
using DesignPattern.Domain.ValueObjects.Seo;

namespace DesignPattern.Domain.Common;

public class Seo
{
  public MetaTitle? Title { get; protected set; }
  public MetaDescription? Description { get; protected set; }
  public Slug? Slug { get; protected set; } // hiển thị trên URL, nếu null thì tự động tạo từ Title

  internal Seo() { }

  private Seo(MetaTitle? title, MetaDescription? description, Slug? slug)
  {
    Title = title;
    Description = description;
    Slug = slug;
  }

  public static Result<Seo> Create(string? metaTitle, string? metaDescription, string? slug)
  {
    var titleResult = NullOrValue.CreateOptional(metaTitle, MetaTitle.Create);
    if (titleResult.IsFailure) return Result<Seo>.Failure(titleResult.Error);

    var descriptionResult = NullOrValue.CreateOptional(metaDescription, MetaDescription.Create);
    if (descriptionResult.IsFailure) return Result<Seo>.Failure(descriptionResult.Error);

    var slugResult = NullOrValue.CreateOptional(slug, Slug.Create);
    if (slugResult.IsFailure) return Result<Seo>.Failure(slugResult.Error);

    var seo = new Seo(titleResult.Value, descriptionResult.Value, slugResult.Value);
    return Result<Seo>.Success(seo);
  }

  public static Seo Rehydrate(string? metaTitle, string? metaDescription, string? slug)
  {
    if (metaTitle is string mt && metaDescription is string md && slug is string s)
    {
      var metaTitleValue = MetaTitle.Rehydrate(mt);
      var metaDescriptionValue = MetaDescription.Rehydrate(md);
      var slugValue = Slug.Rehydrate(s);
      return new Seo(metaTitleValue, metaDescriptionValue, slugValue);
    }
    return new Seo();
  }

  public Result Update(string? metaTitle, string? metaDescription, string? slug)
  {
    if (metaTitle != null)
    {
      var titleResult = MetaTitle.Create(metaTitle);
      if (titleResult.IsFailure) return Result.Failure(titleResult.Error);
      Title = titleResult.Value;
    }

    if (metaDescription != null)
    {
      var descriptionResult = MetaDescription.Create(metaDescription);
      if (descriptionResult.IsFailure) return Result.Failure(descriptionResult.Error);
      Description = descriptionResult.Value;
    }

    if (slug != null)
    {
      var slugResult = Slug.Create(slug);
      if (slugResult.IsFailure) return Result.Failure(slugResult.Error);
      Slug = slugResult.Value;
    }

    return Result.Success();
  }

  public override string ToString()
  {
    return $"[Title]: {Title?.Value}\n[Description]: {Description?.Value}\n[Slug]: {Slug?.Value}";
  }

}
