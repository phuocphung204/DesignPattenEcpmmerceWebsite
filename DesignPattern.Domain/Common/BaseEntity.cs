
using System.Reflection;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects.BaseEntity;

namespace DesignPattern.Domain.Common;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; } = Guid.CreateVersion7(); // Sử dùng UUID phiên bản 7 để đảm bảo tính duy nhất và khả năng sắp xếp theo thời gian
  public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; protected set; }

  protected BaseEntity() { }

}