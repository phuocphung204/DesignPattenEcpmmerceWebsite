using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal class WarehouseItemDocument : BaseDocument
{
  [BsonElement("warehouseId")]
  [BsonRequired]
  public Guid WarehouseId { get; init; }

  [BsonElement("warehouseName")]
  [BsonRequired]
  public required string WarehouseName { get; init; }

  [BsonElement("productId")]
  [BsonRequired]
  public Guid ProductId { get; init; }

  [BsonElement("productName")]
  [BsonRequired]
  public required string ProductName { get; init; }

  [BsonElement("sku")]
  [BsonRequired]
  public required string Sku { get; init; }

  [BsonElement("variantGroupId")]
  [BsonRequired]
  public required string VariantGroupId { get; init; }

  [BsonElement("quantity")]
  public int Quantity { get; init; }

  [BsonElement("waitingForDelivery")]
  public int WaitingForDelivery { get; init; } = 0;
}