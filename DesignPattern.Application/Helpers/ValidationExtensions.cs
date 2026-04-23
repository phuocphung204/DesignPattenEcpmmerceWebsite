using FluentValidation;

namespace DesignPattern.Application.Helpers;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string?> IsNullOrValidGuid<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(value => string.IsNullOrWhiteSpace(value) || Guid.TryParse(value, out _));
  }
}
