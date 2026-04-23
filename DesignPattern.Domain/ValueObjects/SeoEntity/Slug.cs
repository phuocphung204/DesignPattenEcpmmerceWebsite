using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects.SeoEntity;

public sealed class Slug
{
  public string Value { get; init; }
  private Slug(string value)
  {
    Value = value;
  }

  public static Result<Slug> Create(string value)
  {
    // TODO: kiểm tra slug hợp lệ, ví dụ: không chứa ký tự đặc biệt, không có khoảng trắng, v.v. Để đơn giản, chúng ta chỉ kiểm tra xem nó có rỗng hay quá dài hay không. 
    if (string.IsNullOrWhiteSpace(value))
    {
      return Error.Validation(
        "Domain.Slug.Empty",
        "The slug cannot be empty."
      );
    }
    if (value.Length > 255)
    {
      return Error.Validation(
        "Domain.Slug.TooLong",
        "The slug cannot exceed 255 characters."
      );
    }
    return new Slug(value);
  }
  public static Slug Rehydrate(string value)
  {
    return new Slug(value);
  }
}