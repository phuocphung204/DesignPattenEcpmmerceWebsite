using MongoDB.Bson.Serialization.Attributes;
using DesignPattern.Domain.Enums;
using MongoDB.Bson;

namespace DesignPattern.Infrastructure.Mongo.Documents;

internal class ProductDocument : BaseDocument
{
  [BsonElement("name")]
  [BsonRequired]
  public required string Name { get; init; }

  [BsonElement("brandId")]
  public Guid BrandId { get; init; }

  [BsonElement("brandName")]
  public required string BrandName { get; init; }

  [BsonElement("categoryId")]
  public Guid CategoryId { get; init; }

  [BsonElement("categoryName")]
  public required string CategoryName { get; init; }

  [BsonElement("variantGroupId")]
  public string VariantGroupId { get; init; } = string.Empty;

  [BsonElement("sku")]
  [BsonRequired]
  public required string Sku { get; init; }

  [BsonElement("images")]
  public List<string> Images { get; init; } = new();

  [BsonElement("attributes")]
  public List<ProductAttributeDocument> Attributes { get; init; } = new();

  [BsonElement("sellingPrice")]
  public decimal SellingPrice { get; init; }

  [BsonElement("purchasePrice")]
  public decimal PurchasePrice { get; init; }

  [BsonElement("soldQuantity")]
  public int SoldQuantity { get; init; }

  [BsonElement("stockQuantity")]
  public int StockQuantity { get; init; }

  [BsonElement("rating")]
  public decimal Rating { get; init; }

  [BsonElement("shortDescription")]
  public string ShortDescription { get; init; } = string.Empty;

  [BsonElement("detailDescription")]
  public string DetailDescription { get; init; } = string.Empty;

  [BsonElement("status")]
  [BsonRepresentation(BsonType.String)]
  public ProductStatus Status { get; init; } = ProductStatus.Active;
  internal sealed class ProductAttributeDocument
  {
    [BsonElement("name")]
    public string Name { get; init; } = string.Empty;

    [BsonElement("value")]
    public string Value { get; init; } = string.Empty;

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    public ProductAttributeType Type { get; init; }
  }

}