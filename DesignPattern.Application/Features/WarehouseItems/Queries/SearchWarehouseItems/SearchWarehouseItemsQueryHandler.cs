using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.SearchWarehouseItems;

public sealed class SearchWarehouseItemsQueryHandler : IRequestHandler<SearchWarehouseItemsQuery, Result<List<WarehouseItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public SearchWarehouseItemsQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<WarehouseItemResponse>>> Handle(SearchWarehouseItemsQuery request, CancellationToken cancellationToken)
  {
    var warehouseItems = await _unitOfWork.WarehouseItemRepository.SearchAsync(new WarehouseItemSearchCriteria
    {
      WarehouseId = request.WarehouseId,
      ProductName = request.ProductName,
      Sku = request.Sku,
      VariantGroupId = request.VariantGroupId,
      ProductId = request.ProductId
    }, cancellationToken);

    var response = warehouseItems.Select(item => new WarehouseItemResponse
    {
      Id = item.Id,
      WarehouseId = item.WarehouseId,
      WarehouseName = item.WarehouseName.Value,
      ProductId = item.ProductId,
      ProductName = item.ProductName.Value,
      Sku = item.Sku.Value,
      VariantGroupId = item.VariantGroupId.Value,
      Quantity = item.Quantity.Value,
      WaitingForDelivery = item.WaitingForDelivery.Value
    }).ToList();

    return Result<List<WarehouseItemResponse>>.Success(response);
  }
}
