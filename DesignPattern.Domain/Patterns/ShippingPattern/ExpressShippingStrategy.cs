using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Patterns.ShippingPattern;

public class ExpressShippingStrategy : IShippingStrategy
{
  public Price CalculateShippingFee()
  {
    // Implement the cost calculation logic for express shipping
    return Price.Create(20000m).Value; // 20.000 VND
  }
}