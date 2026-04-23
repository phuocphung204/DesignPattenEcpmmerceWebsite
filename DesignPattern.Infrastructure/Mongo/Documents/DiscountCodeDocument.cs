using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public sealed class DiscountCodeDocument : BaseDocument
{
  [BsonElement("code")]
  [BsonRequired]
  public required string Code { get; init; }

  [BsonElement("minimumOrderAmount")]
  [BsonRequired]
  public required decimal MinimumOrderAmount { get; init; }

  [BsonElement("usageLimit")]
  [BsonRequired]
  public required int UsageLimit { get; init; }

  [BsonElement("usedCount")]
  public int UsedCount { get; init; }

  [BsonElement("expirationDate")]
  [BsonRequired]
  public required DateTime ExpirationDate { get; init; }

  [BsonElement("isActive")]
  public bool IsActive { get; init; }

  [BsonElement("type")]
  [BsonRepresentation(BsonType.String)]
  public DiscountType Type { get; init; }

  [BsonElement("amount")]
  public decimal? Amount { get; init; } = null;

  [BsonElement("percent")]
  public decimal? Percent { get; init; } = null;

  [BsonElement("maximumDiscountAmount")]
  public decimal? MaximumDiscountAmount { get; init; } = null;
}
