using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects;

public sealed record class StandardText
{
  public string Value { get; init; }

  private StandardText(string value)
  {
    Value = value.Trim(); // StandardText dùng cho mã sku, groupName ... dài nhất 64 ký tự nên sẽ không chứa khoảng trắng ở đầu và cuối, 
    // nhưng có thể chứa khoảng trắng ở giữa. Do đó, ta sẽ trim giá trị đầu vào để loại bỏ khoảng trắng ở đầu và cuối.
  }
  public static Result<StandardText> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Error.Validation(
        "Domain.StandardText.Empty",
        "The text cannot be empty."
      );
    }
    if (value.Length > 64)
    {
      return Error.Validation(
        "Domain.StandardText.TooLong",
        "The text cannot be longer than 64 characters."
      );
    }
    return new StandardText(value);
  }
  public static StandardText Rehydrate(string value)
  {
    return new StandardText(value);
  }
}