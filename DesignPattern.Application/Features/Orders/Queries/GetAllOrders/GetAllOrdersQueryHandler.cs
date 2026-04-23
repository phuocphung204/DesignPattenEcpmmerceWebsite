using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Application.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<List<ListOrdersResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<ListOrdersResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
  {
    var orders = await _unitOfWork.OrderRepository.GetAllAsync(cancellationToken);
    var orderResponses = ListOrdersResponseMapper.MapToListOrdersResponse(orders);
    return orderResponses;
  }
}