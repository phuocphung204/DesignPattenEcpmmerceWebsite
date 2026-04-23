using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetListByProductId;

public class GetListByProductIdQueryHandler : IRequestHandler<GetListByProductIdQuery, Result<List<WarehouseItemResponse>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetListByProductIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<List<WarehouseItemResponse>>> Handle(GetListByProductIdQuery request, CancellationToken cancellationToken)
  {
    var warehouseItems = await _unitOfWork.WarehouseItemRepository.GetListWarehouseItemByProductIdAsync(request.ProductId, cancellationToken);
    if (warehouseItems.Count == 0)
    {
      return WarehouseErrors.WarehouseItemNotFound;
    }
    return warehouseItems.Select(item => new WarehouseItemResponse
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
  }
}