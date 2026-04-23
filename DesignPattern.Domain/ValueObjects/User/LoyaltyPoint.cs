using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects.User;

public sealed record LoyaltyPoint
{
  public int Value { get; init; }
  private LoyaltyPoint(int value) => Value = value;

  public static Result<LoyaltyPoint> Create(int value)
  {
    return Result<LoyaltyPoint>.From(new LoyaltyPoint(value))
        .Ensure(x => x.Value >= 0, Error.Validation("Domain.LoyaltyPoint.Negative", "Loyalty points cannot be negative."));
  }

  public static LoyaltyPoint Default => new LoyaltyPoint(0);
  public static LoyaltyPoint Rehydrate(int value) => new LoyaltyPoint(value);
}