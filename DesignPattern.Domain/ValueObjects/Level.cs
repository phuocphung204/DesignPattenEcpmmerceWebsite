using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

/// <summary>
/// Level indicates the depth of a category in the category hierarchy.
/// From 0 (root category) to 1 (sub-sub-category). 
/// This allows for a maximum of 2 levels of categories.
/// </summary>
public sealed class Level
{
  public int Value { get; init; }

  private Level(int value)
  {
    Value = value;
  }

  public static Result<Level> Create(int value)
  {
    return Result<Level>.From(new Level(value))
      .Ensure(level => level.Value >= 0, Error.Validation("Domain.Level.Negative", "The level cannot be negative."))
      .Ensure(level => level.Value <= 1, Error.Validation("Domain.Level.TooHigh", "The level cannot exceed 1."));
  }

  public static Level Rehydrate(int value)
  {
    return new Level(value);
  }
}