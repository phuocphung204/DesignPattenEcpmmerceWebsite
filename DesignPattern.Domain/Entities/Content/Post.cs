using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.ValueObjects.Seo;

namespace DesignPattern.Domain.Entities.Content;

public class Post : BaseEntity, ISeoMetadata
{
  public string Title { get; private set; }
  public string Content { get; private set; }
  public string Summary { get; private set; }
  public string ThumbnailUrl { get; private set; }
  public Guid PostCategoryId { get; private set; }
  public string Status { get; private set; }

  public Slug? Slug { get; private set; }
  public MetaTitle? MetaTitle { get; private set; }
  public MetaDescription? MetaDescription { get; private set; }
  public string? MetaKeywords { get; private set; }

  public PostCategory? Category { get; private set; }

  private Post(Guid id, string title, string content, string summary, string thumbnailUrl, Guid postCategoryId, string status)
  {
    Id = id;
    Title = title;
    Content = content;
    Summary = summary;
    ThumbnailUrl = thumbnailUrl;
    PostCategoryId = postCategoryId;
    Status = status;
  }

  public static Result<Post> Create(string title, string content, string summary, string thumbnailUrl, string postCategoryId, string status = "Draft")
  {
    if (!Guid.TryParse(postCategoryId, out var parsedCategoryId))
    {
      return Error.Validation("Post.PostCategoryId.Invalid", "Post category id is invalid.");
    }

    return new Post(Guid.NewGuid(), title, content, summary, thumbnailUrl, parsedCategoryId, status);
  }
}