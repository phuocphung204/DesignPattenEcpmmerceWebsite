using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Domain.Repositories;
using MediatR;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetPage;

public sealed class GetWarehouseItemsPageQueryHandler : IRequestHandler<GetWarehouseItemsPageQuery, PagedResult<WarehouseItemsReadModel>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetWarehouseItemsPageQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResult<WarehouseItemsReadModel>> Handle(GetWarehouseItemsPageQuery request, CancellationToken cancellationToken)
  {
    var pagedWarehouseItems = await _unitOfWork.WarehouseItemRepository.GetPagedAsync(
      new WarehouseItemSearchCriteria
      {
        WarehouseId = request.WarehouseId,
        ProductName = request.ProductName,
        Sku = request.Sku,
        VariantGroupId = request.VariantGroupId,
        ProductId = request.ProductId
      },
      request.PageIndex,
      request.PageSize,
      cancellationToken);
    var warehouseItemsReadModels = pagedWarehouseItems.Items.Select(item => new WarehouseItemsReadModel(
      item.Id,
      item.ProductName.Value,
      item.Sku.Value,
      item.VariantGroupId.Value,
      item.Quantity.Value
    )).ToList();

    return new PagedResult<WarehouseItemsReadModel>
    {
      Items = warehouseItemsReadModels,
      TotalCount = pagedWarehouseItems.TotalCount,
      PageIndex = pagedWarehouseItems.PageIndex,
      PageSize = pagedWarehouseItems.PageSize
    };
  }
}
