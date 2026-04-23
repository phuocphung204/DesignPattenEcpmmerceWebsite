using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects;

public sealed class EmailAddress
{
  public string Value { get; init; }

  private EmailAddress(string value)
  {
    Value = value;
  }

  public static Result<EmailAddress> Create(string value)
  {
    return Result<EmailAddress>.From(new EmailAddress(value))
      .Ensure(email => !string.IsNullOrWhiteSpace(email.Value), Error.Validation("Domain.EmailAddress.Empty", "The email address cannot be empty."))
      .Ensure(email => IsValidEmail(email.Value), Error.Validation("Domain.EmailAddress.InvalidFormat", "The email address format is invalid."));
  }

  private static bool IsValidEmail(string email)
  {
    // TODO: Implement email validation logic here
    return true; // Giả sử luôn hợp lệ cho ví dụ này
  }
  public static EmailAddress Rehydrate(string value)
  {
    return new EmailAddress(value);
  }
}