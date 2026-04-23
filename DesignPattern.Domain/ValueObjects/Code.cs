using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects;

public sealed record class Code
{
  private static readonly int LENGTH = 5;

  public string Value { get; init; }

  private Code(string value)
  {
    Value = value;
  }

  public static Result<Code> Create(string value)
  {
    if (value.Length != LENGTH)
    {
      return Error.Validation(
        "Domain.Code.InvalidLength",
        $"The code must be exactly {LENGTH} characters long.");
    }
    if (!value.All(char.IsLetterOrDigit))
    {
      return Error.Validation(
        "Domain.Code.InvalidCharacters",
        "The code must be alphanumeric.");
    }
    return new Code(value.ToUpperInvariant());
  }

  public static Code Rehydrate(string value)
  {
    return new Code(value);
  }
}
