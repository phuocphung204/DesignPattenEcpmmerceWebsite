using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Enums;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Abstractions;

namespace DesignPattern.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEnumerable<IOrderObserver> _orderObservers;

  public ConfirmOrderCommandHandler(IUnitOfWork unitOfWork, IEnumerable<IOrderObserver> orderObservers)
  {
    _unitOfWork = unitOfWork;
    _orderObservers = orderObservers;
  }

  public async Task<Result<OrderResponse>> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
  {
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId, cancellationToken);
    if (order is null)
      return OrderErrors.OrderNotFound;

    foreach (var observer in _orderObservers)
      order.Attach(observer);

    var result = order.ConfirmOrder(request.UserId);
    if (result.IsFailure)
      return result.Error;

    _unitOfWork.OrderRepository.Update(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    await order.NotifyAsync(); // Thông báo sau khi đã lưu thay đổi vào database để đảm bảo các observer nhận được thông tin mới nhất
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var orderResponseResult = OrderResponseMapper.MapToOrderResponse(order);
    if (orderResponseResult.IsFailure)
      return orderResponseResult.Error;

    return orderResponseResult.Value;
  }
}