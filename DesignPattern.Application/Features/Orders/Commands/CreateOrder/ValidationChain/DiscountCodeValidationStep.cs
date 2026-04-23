using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class DiscountCodeValidationStep : CreateOrderValidationStepBase
{
  protected override async Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var discountCodeResult = await handler.ValidateAndGetDiscountCode(context.Dto.DiscountCode);
    if (discountCodeResult.IsFailure)
      return discountCodeResult.Error;

    context.DiscountCode = discountCodeResult.Value;
    return Result.Success();
  }
}
