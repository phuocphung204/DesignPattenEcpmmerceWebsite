using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects;

public sealed class RatingValue
{
  public int Value { get; init; }
  public static RatingValue Zero => new RatingValue(0);

  private RatingValue(int value)
  {
    Value = value;
  }

  public static Result<RatingValue> Create(int value)
  {
    if (value < 1 || value > 5)
    {
      return Error.Validation(
        "Domain.RatingValue.InvalidRange",
        "Rating value must be between 1 and 5."
      );
    }
    return new RatingValue(value);
  }

  public static RatingValue Rehydrate(int value)
  {
    return new RatingValue(value);
  }
}