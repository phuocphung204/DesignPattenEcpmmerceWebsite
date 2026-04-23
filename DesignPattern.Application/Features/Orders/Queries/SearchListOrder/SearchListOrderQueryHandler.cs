using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.Orders.Shared;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.Orders.Queries.SearchListOrder;

public class SearchListOrderQueryHandler : IRequestHandler<SearchListOrderQuery, Result<PagedResult<ListOrdersResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public SearchListOrderQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<PagedResult<ListOrdersResponse>>> Handle(SearchListOrderQuery request, CancellationToken cancellationToken)
  {
    var pagedOrders = await _unitOfWork.OrderRepository.SearchListOrderAsync(
      request.Start,
      request.End,
      request.IdSearch,
      request.PageIndex,
      request.PageSize,
      cancellationToken);

    var mapped = new PagedResult<ListOrdersResponse>
    {
      Items = ListOrdersResponseMapper.MapToListOrdersResponse(pagedOrders.Items),
      TotalCount = pagedOrders.TotalCount,
      PageIndex = pagedOrders.PageIndex,
      PageSize = pagedOrders.PageSize
    };

    return mapped;
  }
}
