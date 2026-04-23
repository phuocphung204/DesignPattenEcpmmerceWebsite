using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal abstract class CreateOrderValidationStepBase : ICreateOrderValidationStep
{
  private ICreateOrderValidationStep? _next;

  public ICreateOrderValidationStep SetNext(ICreateOrderValidationStep next)
  {
    _next = next;
    return next;
  }

  public async Task<Result> HandleAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var currentResult = await ExecuteAsync(context, cancellationToken, handler);
    if (currentResult.IsFailure)
      return currentResult.Error;

    if (_next is null)
      return Result.Success();

    return await _next.HandleAsync(context, cancellationToken, handler);
  }

  protected abstract Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler);
}
