using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects;

public class Percent
{
  public decimal Value { get; }

  public static Percent TenPercent => new Percent(10);
  public static Percent TwentyPercent => new Percent(20);

  private Percent(decimal value)
  {
    Value = value;
  }

  public static Result<Percent> Create(decimal value)
  {
    if (value < 0 || value > 100)
      return Error.Validation(
        "Domain.Percent.InvalidValue",
        "Percent must be between 0 and 100."
      );

    return new Percent(value);
  }

  public static Percent Rehydrate(decimal value)
  {
    return new Percent(value);
  }

  public static explicit operator decimal(Percent percent) => percent.Value / 100;

  public Price ApplyToPrice(Price price)
  {
    Price discountAmount = this * price;

    return discountAmount;
  }
  public Quantity ApplyToQuantity(Price price)
  {
    Quantity discountAmount = Quantity.Create((int)(this * price).Amount).Value;

    return discountAmount;
  }


  public static Price operator *(Price price, Percent percent)
  => Price.Create(price.Amount * percent.Value).Value;
  public static Price operator *(Percent percent, Price price)
    => Price.Create(price.Amount * percent.Value).Value;

}
