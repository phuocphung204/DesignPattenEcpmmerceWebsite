using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Repositories;
namespace DesignPattern.Application.Features.Orders.Queries.GetOrdersByUserId;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Result<List<ListOrdersResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;
  public GetOrdersQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<List<ListOrdersResponse>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
  {
    var orders = await _unitOfWork.OrderRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    var response = ListOrdersResponseMapper.MapToListOrdersResponse(orders);
    return response;
  }
}