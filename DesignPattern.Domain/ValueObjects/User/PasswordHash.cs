using System.Security.Cryptography;
using System.Text;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.ValueObjects.User;

public sealed class PasswordHash
{
  private const int PASSWORD_MIN_LENGTH = 8;
  public string Value { get; init; }

  private PasswordHash(string value) => Value = value;

  public static Result<PasswordHash> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Error.Validation(
        "Domain.PasswordHash.Empty",
        "PasswordHash cannot be empty."
      );
    }

    if (value.Length < PASSWORD_MIN_LENGTH)
    {
      return Error.Validation(
        "Domain.PasswordHash.TooShort",
        $"PasswordHash must be at least {PASSWORD_MIN_LENGTH} characters long."
      );
    }

    return Result<PasswordHash>.Success(new PasswordHash(HashSHA256(value)));
  }

  public static string HashSHA256(string text)
  {
    using (SHA256 sha256 = SHA256.Create())
    {
      byte[] bytes = Encoding.UTF8.GetBytes(text);
      byte[] hash = sha256.ComputeHash(bytes);

      return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
  }

  public static PasswordHash Rehydrate(string value)
  {
    return new PasswordHash(value);
  }
}
