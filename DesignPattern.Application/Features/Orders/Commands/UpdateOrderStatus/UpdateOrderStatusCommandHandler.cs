using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateStatusDeliveryCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateStatusDeliveryCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<OrderResponse>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    // check order exists
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
    if (order == null)
      return OrderErrors.OrderNotFound;

    // check user exists to set in order history
    var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
    if (user == null)
      return OrderErrors.InvalidUserId;

    var now = DateTime.UtcNow;
    var transitionResult = order.TryTransitionStatus(
      dto.NewStatus,
      BuildStatusNote(dto.NewStatus, now, dto.Note),
      user.Id,
      user.Role);
    if (transitionResult.IsFailure)
      return transitionResult.Error;

    if (dto.NewStatus == OrderStatusEnum.Delivered && order.PaymentInfo.Type == PaymentType.Cash && order.PaymentStatus != PaymentStatusEnum.Paid)
    {
      order.SetPaymentStatus(PaymentStatusEnum.Paid);
      order.PaymentInfo.SetPaidAt(now);
    }

    _unitOfWork.OrderRepository.Update(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return OrderResponseMapper.MapToOrderResponse(order);
  }

  private static string BuildStatusNote(OrderStatusEnum status, DateTime changedAt, string? note)
  {
    if (!string.IsNullOrWhiteSpace(note))
      return note;

    return status switch
    {
      OrderStatusEnum.Shipped => $"Đơn hàng đã được giao cho đơn vị vận chuyển vào ngày {changedAt:dd/MM/yyyy HH:mm:ss}",
      OrderStatusEnum.Delivered => $"Đơn hàng đã được giao thành công vào ngày {changedAt:dd/MM/yyyy HH:mm:ss}",
      OrderStatusEnum.Completed => $"Đơn hàng đã hoàn thành vào ngày {changedAt:dd/MM/yyyy HH:mm:ss}",
      _ => "Cập nhật trạng thái đơn hàng"
    };
  }
}