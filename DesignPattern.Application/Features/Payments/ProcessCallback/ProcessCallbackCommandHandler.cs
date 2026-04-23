using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Payments.ProcessCallback;

public class ProcessCallbackCommandHandler : IRequestHandler<ProcessCallbackCommand, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public ProcessCallbackCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<OrderResponse>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(dto.OrderCode, cancellationToken);
    if (order is null)
    {
      return OrderErrors.OrderNotFound;
    }
    if (!dto.IsSuccess)
    {
      return PaymentErrors.PaymentFailed;
    }
    if (order.PaymentStatus == PaymentStatusEnum.Paid)
    {
      return OrderErrors.AlreadyPaid;
    }
    order.SetPaymentStatus(PaymentStatusEnum.Paid);
    var paymentInfo = order.PaymentInfo.MarkAsPaid(
      dto.Provider,
      dto.Channel,
      dto.TransactionId,
      dto.PaidAt
    );

    _unitOfWork.OrderRepository.Update(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return OrderResponseMapper.MapToOrderResponse(order);
  }
}