using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal sealed class RatingDocument : BaseDocument
{
  [BsonElement("productId")]
  [BsonRequired]
  public Guid ProductId { get; init; }

  [BsonElement("userId")]
  [BsonRequired]
  public Guid UserId { get; init; }

  [BsonElement("value")]
  public int Value { get; init; } = 0;

  [BsonElement("comment")]
  public string Comment { get; init; }
}