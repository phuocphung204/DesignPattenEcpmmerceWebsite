using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

public class Name
{
  public string Value { get; init; }
  private Name(string value)
  {
    Value = value;
  }
  public static Result<Name> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Result<Name>.Failure(Error.Validation(
        "Domain.Name.Empty",
        "The name cannot be empty."));
    }
    if (value.Length > 100)
    {
      return Error.Validation(
        "Domain.Name.TooLong",
        "The name cannot exceed 100 characters.");
    }
    return new Name(value);
  }
  public static Name Rehydrate(string value)
  {
    return new Name(value);
  }
}
