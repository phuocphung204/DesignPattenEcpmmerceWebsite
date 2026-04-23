using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public class ProductCategoryDocument : BaseDocument
{

  [BsonElement("name")]
  [BsonRequired]
  public string Name { get; init; } = string.Empty;

  [BsonElement("level")]
  public int Level { get; init; }

  [BsonElement("parentCategoryId")]
  public Guid? ParentCategoryId { get; init; }

}
