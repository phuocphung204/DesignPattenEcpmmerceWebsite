using DesignPattern.Domain.Errors;
using FluentValidation;
using MediatR;

namespace DesignPattern.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
  : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly IEnumerable<IValidator<TRequest>> _validators;

  public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
  {
    _validators = validators;
  }

  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    if (!_validators.Any())
    {
      return await next(cancellationToken);
    }

    // tạo ValidationContext từ request
    var context = new ValidationContext<TRequest>(request);

    // chay tất cả validator song song và thu thập kết quả
    var validationResults = await Task.WhenAll(
      _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

    // gôm lỗi
    var failures = validationResults
      .SelectMany(result => result.Errors)
      .Where(error => error is not null)
      .DistinctBy(error => error.ErrorMessage)
      .ToArray();

    if (failures.Length == 0)
    {
      return await next(cancellationToken);
    }

    var message = string.Join("; ", failures.Select(error => error.ErrorMessage));
    var errorResult = Error.Validation("Validation.Error", message);

    (bool shouldThrow, TResponse failureValue) = CreateFailureResponse.Create<TResponse>(errorResult);
    if (!shouldThrow)
    {
      return failureValue;
    }

    throw new ValidationException(failures);
  }

}
