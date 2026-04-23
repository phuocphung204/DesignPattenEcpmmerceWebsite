using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class ShippingInfoValidationStep : CreateOrderValidationStepBase
{
  protected override async Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var shippingInfoResult = await handler.ValidateUserLoyaltyPointsAndCreateShippingInfo(
      context.UserId,
      context.Dto.ShippingInfo,
      context.PointsToRedeem);

    if (shippingInfoResult.IsFailure)
      return shippingInfoResult.Error;

    context.ShippingInfo = shippingInfoResult.Value;
    return Result.Success();
  }
}
