using DesignPattern.Domain.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public class BrandDocument : BaseDocument
{
  [BsonElement("name")]
  [BsonRequired]
  public required string Name { get; init; }

  [BsonElement("level")]
  public int Level { get; init; }

  [BsonElement("parentId")]
  public Guid? ParentId { get; init; }

}
