using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects.Seo;

public sealed class MetaTitle
{
  public string Value { get; init; }

  private MetaTitle(string value)
  {
    Value = value;
  }

  public static Result<MetaTitle> Create(string value)
  {
    return Result<MetaTitle>.From(new MetaTitle(value))
      .Ensure(title => !string.IsNullOrWhiteSpace(title.Value), Error.Validation("Domain.MetaTitle.Empty", "The title cannot be empty."))
      .Ensure(title => title.Value.Length <= 255, Error.Validation("Domain.MetaTitle.TooLong", "The title cannot exceed 255 characters."));
  }
  public static MetaTitle Rehydrate(string value)
  {
    return new MetaTitle(value);
  }
}
