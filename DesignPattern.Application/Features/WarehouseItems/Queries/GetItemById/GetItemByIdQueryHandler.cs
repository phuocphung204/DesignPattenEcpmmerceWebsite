using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.WarehouseItems.Queries.GetItemById;

public sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<WarehouseItemResponse?>>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetItemByIdQueryHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<WarehouseItemResponse?>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
  {
    var item = await _unitOfWork.WarehouseItemRepository.GetByIdAsync(request.Id, cancellationToken);
    if (item is null)
    {
      return Result<WarehouseItemResponse?>.Failure(WarehouseErrors.WarehouseItemNotFound);
    }
    var response = new WarehouseItemResponse
    {
      Id = item.Id,
      WarehouseId = item.WarehouseId,
      WarehouseName = item.WarehouseName.Value,
      ProductId = item.ProductId,
      ProductName = item.ProductName.Value,
      Sku = item.Sku.Value,
      VariantGroupId = item.VariantGroupId.Value,
      Quantity = item.Quantity.Value
    };
    return Result<WarehouseItemResponse?>.Success(response);
  }
}