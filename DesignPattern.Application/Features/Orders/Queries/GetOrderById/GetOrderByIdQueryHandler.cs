using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
  {
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId, cancellationToken);
    if (order is null)
      return OrderErrors.OrderNotFound;

    if (order.UserId != request.UserId)
      return OrderErrors.OrderNotFound;

    var orderResponseResult = OrderResponseMapper.MapToOrderResponse(order);
    if (orderResponseResult.IsFailure)
      return Result<OrderResponse>.Failure(orderResponseResult.Error);

    return orderResponseResult.Value;
  }
}