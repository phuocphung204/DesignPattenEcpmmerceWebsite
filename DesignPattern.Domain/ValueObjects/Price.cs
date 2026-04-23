using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

public sealed record class Price
{
  public decimal Amount { get; init; }
  public static Price Zero => new Price(0);

  private Price(decimal amount)
  {
    Amount = amount;
  }

  public static Result<Price> Create(decimal amount)
  {
    if (amount < 0)
      return Error.Validation(
        "Domain.Price.NegativeAmount",
        "Amount cannot be negative."
      );

    return new Price(amount);
  }

  public static Price Rehydrate(decimal amount)
  {
    return new Price(amount);
  }

  public Price Add(Price other)
  {
    return new Price(this.Amount + other.Amount);
  }

  public Price Subtract(Price other)
  {
    decimal result = this.Amount - other.Amount;

    return new Price(result);
  }

  public Price Multiply(Quantity factor)
  {
    return new Price(this.Amount * factor.Value);
  }

  public Price Divide(Quantity divisor)
  {
    return new Price(this.Amount / divisor.Value);
  }

  public static Price Min(Price price1, Price price2)
  {
    return new Price(Math.Min(price1.Amount, price2.Amount));
  }

  public static Price Max(Price price1, Price price2)
  {
    return new Price(Math.Max(price1.Amount, price2.Amount));
  }

  /// <summary>
  /// Operator overloading for Price
  /// </summary>
  /// <remarks>
  /// <code>
  /// var price1 = Price.Create(10).Value;
  /// var price2 = Price.Create(20).Value;
  /// var result = price1 + price2;
  /// </code>
  /// </remarks>
  /// <param name="price1">The first price.</param>
  /// <param name="price2">The second price.</param>
  /// <returns>The sum of the two prices.</returns>
  public static Price operator +(Price price1, Price price2) => new(price1.Amount + price2.Amount);

  public static Price operator -(Price price1, Price price2) => new(price1.Amount - price2.Amount);

  public static Price operator *(Price price1, Price price2) => new(price1.Amount * price2.Amount);

  public static Price operator /(Price price1, Price price2) => new(price1.Amount / price2.Amount);


  public static bool operator <(Price price1, Price price2) => price1.Amount < price2.Amount;

  public static bool operator >(Price price1, Price price2) => price1.Amount > price2.Amount;

  public static bool operator <=(Price price1, Price price2) => price1.Amount <= price2.Amount;

  public static bool operator >=(Price price1, Price price2) => price1.Amount >= price2.Amount;

}
