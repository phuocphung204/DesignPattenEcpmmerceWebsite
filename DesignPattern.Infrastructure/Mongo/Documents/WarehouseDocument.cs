using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal sealed class WarehouseDocument : BaseDocument
{
  [BsonElement("name")]
  [BsonRequired]
  public required string Name { get; init; }

  [BsonElement("address")]
  [BsonRequired]
  public required string Address { get; init; }

}