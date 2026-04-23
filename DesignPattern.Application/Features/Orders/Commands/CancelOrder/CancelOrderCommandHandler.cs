using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Features.Products.Commands.CreateProduct;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEnumerable<IOrderObserver> _orderObservers;

  public CancelOrderCommandHandler(IUnitOfWork unitOfWork, IEnumerable<IOrderObserver> orderObservers)
  {
    _unitOfWork = unitOfWork;
    _orderObservers = orderObservers;
  }

  public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId, cancellationToken);
    if (order is null)
      return OrderErrors.OrderNotFound;

    var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
    if (user is null)
      return UserErrors.UserNotFound;

    foreach (var observer in _orderObservers)
      order.Attach(observer);

    if (user.Role == UserRole.Customer)
    {
      var cancelResult = order.CancelOrderByCustomer(dto.Note, request.UserId);
      if (cancelResult.IsFailure)
        return cancelResult.Error;
    }
    else if (user.Role == UserRole.Manager)
    {
      var cancelResult = order.CancelOrderByManager(dto.Note, request.UserId);
      if (cancelResult.IsFailure)
        return cancelResult.Error;
    }

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
