using System.Text.RegularExpressions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Extensions;

namespace DesignPattern.Domain.ValueObjects.User;

public sealed class PhoneNumber
{
  public string Value { get; init; }
  // Regex for exactly 10 digits
  private static readonly Regex PhoneRegex = new(@"^\d{10}$", RegexOptions.Compiled);

  private PhoneNumber(string value) => Value = value;

  public static Result<PhoneNumber> Create(string value)
  {
    return Result<PhoneNumber>.From(new PhoneNumber(value))
        .Ensure(x => !string.IsNullOrWhiteSpace(x.Value), Error.Validation("Domain.PhoneNumber.Empty", "Phone number cannot be empty."))
        .Ensure(x => PhoneRegex.IsMatch(x.Value), Error.Validation("Domain.PhoneNumber.InvalidFormat", "Phone number must contain exactly 10 digits."));
  }
  public static PhoneNumber Rehydrate(string value)
  {
    return new PhoneNumber(value);
  }
}