using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Patterns.ShippingPattern;

public interface IShippingStrategy
{
  public Price CalculateShippingFee();
}