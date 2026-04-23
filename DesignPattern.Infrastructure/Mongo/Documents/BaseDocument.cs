using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public class BaseDocument
{
  [BsonId(IdGenerator = typeof(UuidV7IdGenerator))] // 1. Báo đây là Khóa chính + Chỉ định bộ sinh ID tự động
  [BsonGuidRepresentation(GuidRepresentation.Standard)] // 2. Ép lưu dưới dạng Subtype 4 (Chuẩn quốc tế)
  public Guid Id { get; init; }


  [BsonElement("createdAt")]
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

  [BsonElement("updatedAt")]
  public DateTime? UpdatedAt { get; set; }
}