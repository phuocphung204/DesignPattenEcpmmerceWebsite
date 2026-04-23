using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Patterns.ShippingPattern;

public class StandardShippingStrategy : IShippingStrategy
{
  public Price CalculateShippingFee()
  {
    // Implement the cost calculation logic for standard shipping
    return Price.Create(10000m).Value; // 10.000 VND
  }
}