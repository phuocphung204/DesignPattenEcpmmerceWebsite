using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects.BaseEntity;

public sealed class ID
{
  public string Value { get; init; }

  private ID(string value)
  {
    Value = value;
  }

  public static Result<ID> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
      return Error.Validation("Domain.ID.Empty", "ID cannot be empty.");

    return new ID(value);
  }
  public static ID Rehydrate(string value)
  {
    return new ID(value);
  }
}