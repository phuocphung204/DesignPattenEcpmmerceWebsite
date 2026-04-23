using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal sealed class CartDocument : BaseDocument
{
  [BsonElement("userId")]
  [BsonRequired]
  public Guid UserId { get; init; }

  [BsonElement("items")]
  public List<CartItemDocument> Items { get; init; } = new();
}

internal sealed class CartItemDocument
{
  [BsonElement("productId")]
  [BsonRequired]
  public required Guid ProductId { get; init; }

  [BsonElement("imageLink")]
  public string ImageLink { get; init; } = string.Empty;

  [BsonElement("sku")]
  [BsonRequired]
  public required string Sku { get; init; }

  [BsonElement("name")]
  [BsonRequired]
  public required string Name { get; init; }

  [BsonElement("quantity")]
  [BsonRequired]
  public int Quantity { get; init; } = 1;

  [BsonElement("sellingPrice")]
  [BsonRequired]
  public decimal SellingPrice { get; init; }

  [BsonElement("attributes")]
  public List<AttributeDocument> Attributes { get; init; } = new();
}

internal sealed class AttributeDocument
{
  [BsonElement("name")]
  public string Name { get; init; } = string.Empty;

  [BsonElement("value")]
  public string Value { get; init; } = string.Empty;
}
