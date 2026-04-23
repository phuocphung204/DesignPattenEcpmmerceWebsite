using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;

namespace DesignPattern.Application.Features.Payments.ProcessCallback;

public record ProcessCallbackCommand(PaymentResult dto)
  : IRequest<Result<OrderResponse>>;