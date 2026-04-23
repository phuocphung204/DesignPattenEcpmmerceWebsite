using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public class SeoDocument
{
  [BsonId]
  public ObjectId Id { get; } = ObjectId.GenerateNewId();

  [BsonElement("refEntityId")]
  public Guid RefEntityId { get; set; }

  [BsonElement("metaTitle")]
  public string? MetaTitle { get; init; }

  [BsonElement("metaDescription")]
  public string? MetaDescription { get; init; }

  [BsonElement("slug")]
  public string? Slug { get; init; }
}