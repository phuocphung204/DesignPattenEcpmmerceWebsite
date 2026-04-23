using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class PaymentInfoValidationStep : CreateOrderValidationStepBase
{
  protected override Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var paymentInfoResult = handler.ValidateAndCreatePaymentInfo(context.Dto.PaymentInfo);
    if (paymentInfoResult.IsFailure)
      return Task.FromResult<Result>(paymentInfoResult.Error);

    context.PaymentInfo = paymentInfoResult.Value;
    return Task.FromResult(Result.Success());
  }
}
