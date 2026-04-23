using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal sealed class ValueObjectsValidationStep : CreateOrderValidationStepBase
{
  protected override Task<Result> ExecuteAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler)
  {
    var pointsToRedeemResult = Quantity.Create(context.Dto.PointsUsed);
    if (pointsToRedeemResult.IsFailure)
      return Task.FromResult<Result>(pointsToRedeemResult.Error);

    context.PointsToRedeem = pointsToRedeemResult.Value;

    if (context.Dto.DiscountCode is string codeValue)
    {
      var codeResult = Code.Create(codeValue);
      if (codeResult.IsFailure)
        return Task.FromResult<Result>(codeResult.Error);

      context.CodeValue = codeResult.Value;
    }

    if (context.Dto.Note is string note)
    {
      var noteResult = Note.Create(note);
      if (noteResult.IsFailure)
        return Task.FromResult<Result>(new Error("Order.Note.Invalid", "Invalid order note."));

      context.NoteValue = noteResult.Value;
    }

    return Task.FromResult(Result.Success());
  }
}
