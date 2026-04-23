using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class OrderItemsValidationStep : CreateOrderValidationStepBase
{
  protected override async Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var orderItemsResult = await handler.ValidateAvailabilityOfProductsAndCreateOrderItems(context.Dto.Items, cancellationToken);
    if (orderItemsResult.IsFailure)
      return orderItemsResult.Error;

    context.OrderItems = orderItemsResult.Value;
    return Result.Success();
  }
}
