using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Behaviors;

public static class CreateFailureResponse
{
  public static (bool shouldThrow, TResponse failureValue) Create<TResponse>(Error errorResult)
  {
    if (typeof(TResponse) == typeof(Result))
    {
      return (shouldThrow: false, failureValue: (TResponse)(object)Result.Failure(errorResult));
    }

    if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
    {
      // lấy kiểu T trong Result<T>
      var genericType = typeof(TResponse).GetGenericArguments()[0];

      var resultType = typeof(Result<>).MakeGenericType(genericType);
      // sử dụng reflection để gọi phương thức Failure của Result<T>
      var failureMethod = resultType.GetMethod(nameof(Result<object>.Failure), new[] { typeof(Error) });
      if (failureMethod is not null)
      {
        var failure = failureMethod.Invoke(null, new object[] { errorResult });
        if (failure is TResponse typedFailure)
        {
          return (shouldThrow: false, failureValue: typedFailure);
        }
      }
    }

    return (shouldThrow: true, failureValue: default!);
  }
}
