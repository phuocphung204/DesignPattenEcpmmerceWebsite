using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

public sealed record class Quantity
{
  public int Value { get; init; }
  public static Quantity Ten => new Quantity(10);
  public static Quantity Zero => new Quantity(0);
  public static Quantity One => new Quantity(1);

  private Quantity(int value)
  {
    Value = value;
  }

  public static Result<Quantity> Create(int value)
  {
    if (value < 0)
    {
      return Error.Validation(
        "Domain.Quantity.NonPositive",
        "Quantity must be a positive integer."
      );
    }
    return new Quantity(value);
  }

  public static Quantity Rehydrate(int value)
  {
    return new Quantity(value);
  }

  public Quantity Add(Quantity other)
  {
    return new Quantity(this.Value + other.Value);
  }

  public Quantity Subtract(Quantity other)
  {
    if (this.Value - other.Value < 0)
    {
      return new Quantity(0);
    }
    return new Quantity(this.Value - other.Value);
  }

  public static Quantity operator +(Quantity quantity1, Quantity quantity2)
    => new(quantity1.Value + quantity2.Value);

  public static Quantity operator -(Quantity quantity1, Quantity quantity2)
    => new Quantity(quantity1.Value - quantity2.Value).Value < 0 ?
      Zero :
      new Quantity(quantity1.Value - quantity2.Value);

  public static bool operator <(Quantity quantity1, Quantity quantity2)
    => quantity1.Value < quantity2.Value;

  public static bool operator >(Quantity quantity1, Quantity quantity2)
    => quantity1.Value > quantity2.Value;

  public static bool operator <=(Quantity quantity1, Quantity quantity2)
    => quantity1.Value <= quantity2.Value;

  public static bool operator >=(Quantity quantity1, Quantity quantity2)
    => quantity1.Value >= quantity2.Value;

}
