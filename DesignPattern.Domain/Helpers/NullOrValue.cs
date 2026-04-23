using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.Helpers;

internal class NullOrValue
{
  internal static Result<T?> CreateOptional<T, TValue>(TValue? value, Func<TValue, Result<T>> factory)
  {
    if (value == null)
    {
      return Result<T?>.Success(default);
    }

    var result = factory(value);
    if (result.IsFailure)
    {
      return Result<T?>.Failure(result.Error);
    }

    return Result<T?>.Success(result.Value);
  }

}