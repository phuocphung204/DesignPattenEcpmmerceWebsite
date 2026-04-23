using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects.BaseEntity;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.ValueObjects;

public class ProductAttribute
{
  public Name Name { get; private set; }
  public StandardText Value { get; private set; }
  public ProductAttributeType Type { get; private set; }

  private ProductAttribute(Name name, StandardText value, ProductAttributeType type)
  {
    Name = name;
    Value = value;
    Type = type;
  }

  public static Result<ProductAttribute> Create(string name, string value, ProductAttributeType type)
  {
    Result<Name> nameResult = Name.Create(name);
    if (nameResult.IsFailure)
      return nameResult.Error;

    Result<StandardText> valueResult = StandardText.Create(value);
    if (valueResult.IsFailure)
      return valueResult.Error;

    return new ProductAttribute(nameResult.Value, valueResult.Value, type);
  }
  public static ProductAttribute Rehydrate(Name name, StandardText value, ProductAttributeType type)
  {
    return new ProductAttribute(name, value, type);
  }
}
