using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DesignPattern.Domain.ValueObjects.Seo;

public sealed class Slug
{
  private static readonly Regex InvalidSlugCharsRegex = new("[^a-z0-9]+", RegexOptions.Compiled);

  public string Value { get; init; }
  private Slug(string value)
  {
    Value = value;
  }

  public static Result<string> GenerateFromName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return Error.Validation(
        "Domain.Slug.Name.Empty",
        "Cannot generate slug from an empty name."
      );
    }

    var lowered = RemoveDiacritics(name.Trim())
      // chuyển về viết thường
      .ToLowerInvariant();

    // thay thế các ký tự không hợp lệ bằng dấu gạch ngang
    var withHyphen = InvalidSlugCharsRegex.Replace(lowered, "-");

    return withHyphen.Trim('-');
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

  /// <summary>
  /// Hàm này sẽ loại bỏ các dấu tiếng Việt và các ký tự đặc biệt,
  ///  sau đó chuyển về dạng không dấu. 
  /// Ví dụ: "Cà phê sữa đá" sẽ trở thành "ca phe sua da".
  /// </summary>
  /// <param name="value">Chuỗi đầu vào cần xử lý</param>
  /// <returns>Chuỗi sau khi đã loại bỏ dấu và ký tự đặc biệt</returns>
  private static string RemoveDiacritics(string value)
  {
    // Chuyển về dạng tổ hợp NFD
    var normalized = value.Normalize(NormalizationForm.FormD);

    var chars = normalized
      // Loại bỏ các ký tự dấu
      .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
      // Thay thế 'đ' và 'Đ' bằng 'd' và 'D'
      .Select(c => c == 'đ' ? 'd' : c == 'Đ' ? 'D' : c)
      .ToArray();

    // Chuyển về dạng chuẩn NFC
    return new string(chars).Normalize(NormalizationForm.FormC);
  }

}