using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Patterns.ShippingPattern;

public static class ShippingStrategyFactory
{
  public static IShippingStrategy Create(ShippingType type)
  {
    IShippingStrategy _strategy;
    switch (type)
    {
      case ShippingType.Standard:
        _strategy = new StandardShippingStrategy();
        break;

      case ShippingType.Express:
        _strategy = new ExpressShippingStrategy();
        break;

      default:
        throw new InvalidOperationException("Invalid shipping type");
    }
    return _strategy;
  }
}