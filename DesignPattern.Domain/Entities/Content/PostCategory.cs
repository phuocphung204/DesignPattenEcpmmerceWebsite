using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.ValueObjects.Seo;
using EntityId = DesignPattern.Domain.ValueObjects.BaseEntity.ID;

namespace DesignPattern.Domain.Entities.Content;

public class PostCategory : BaseEntity, ISeoMetadata
{
  public Name Name { get; private set; }
  public EntityId? ParentId { get; private set; }

  public Slug? Slug { get; private set; }
  public MetaTitle? MetaTitle { get; private set; }
  public MetaDescription? MetaDescription { get; private set; }
  public string? MetaKeywords { get; private set; }

  public PostCategory? Parent { get; private set; }
  public List<PostCategory> Children { get; private set; } = [];

  public List<Post> Posts { get; private set; } = [];

  private PostCategory(Guid id, Name name, EntityId? parentId)
  {
    // Id = id;
    Name = name;
    ParentId = parentId;
  }

  public static Result<PostCategory> Create(string name, string? parentId = null)
  {
    var nameResult = Name.Create(name);
    if (nameResult.IsFailure) return nameResult.Error;

    EntityId? parsedParentId = null;
    if (!string.IsNullOrEmpty(parentId))
    {
      var parentIdResult = EntityId.Create(parentId);
      if (parentIdResult.IsFailure) return parentIdResult.Error;
      parsedParentId = parentIdResult.Value;
    }

    return new PostCategory(Guid.NewGuid(), nameResult.Value, parsedParentId);
  }
}