using DesignPattern.Domain.Common;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects.Seo;

public sealed class MetaDescription
{
  public string Value { get; init; }

  private MetaDescription(string value)
  {
    Value = value;
  }

  public static Result<MetaDescription> Create(string value)
  {
    if (value == null) return Error.NullValue;
    if (value.Length > 200) return Error.Validation("Domain.MetaDescription.TooLong", "The meta description cannot exceed 200 characters.");
    return Result<MetaDescription>.Success(new MetaDescription(value));
  }

  public static MetaDescription Rehydrate(string value)
  {
    return new MetaDescription(value);
  }
}
