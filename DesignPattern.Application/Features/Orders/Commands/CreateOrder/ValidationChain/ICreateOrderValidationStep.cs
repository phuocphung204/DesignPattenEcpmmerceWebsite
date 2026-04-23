using DesignPattern.Domain.Common;

namespace DesignPattern.Application.Features.Orders.Commands.CreateOrder.ValidationChain;

internal interface ICreateOrderValidationStep
{
  ICreateOrderValidationStep SetNext(ICreateOrderValidationStep next);
  Task<Result> HandleAsync(CreateOrderValidationContext context, CancellationToken cancellationToken, CreateOrderCommandHandler handler);
}
