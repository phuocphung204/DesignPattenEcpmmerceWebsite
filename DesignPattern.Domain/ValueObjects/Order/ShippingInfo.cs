using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects.Order;

public sealed record class ShippingInfo
{
  public ShippingType Type { get; }
  public Address Address { get; }
  private ShippingInfo(ShippingType type, Address address)
  {
    Type = type;
    Address = address;
  }

  public static Result<ShippingInfo> Create(ShippingType type, Address address)
  {

    ShippingInfo shippingInfo = new ShippingInfo(type, address);
    return shippingInfo;
  }

  public ShippingInfo Rehydrate(ShippingType type, Address address)
  {
    return new ShippingInfo(type, address);
  }
}