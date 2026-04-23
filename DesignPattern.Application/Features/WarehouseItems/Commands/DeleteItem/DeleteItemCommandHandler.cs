using MediatR;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result<WarehouseItemResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteItemCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<WarehouseItemResponse>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
  {
    var warehouseItem = await _unitOfWork.WarehouseItemRepository.GetByIdAsync(request.Id, cancellationToken);
    if (warehouseItem is null)
      return Result<WarehouseItemResponse>.Failure(WarehouseErrors.WarehouseItemNotFound);

    _unitOfWork.WarehouseItemRepository.Delete(warehouseItem);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    var response = new WarehouseItemResponse
    {
      Id = warehouseItem.Id,
      WarehouseId = warehouseItem.WarehouseId,
      WarehouseName = warehouseItem.WarehouseName.Value,
      ProductId = warehouseItem.ProductId,
      ProductName = warehouseItem.ProductName.Value,
      Sku = warehouseItem.Sku.Value,
      VariantGroupId = warehouseItem.VariantGroupId.Value,
      Quantity = warehouseItem.Quantity.Value,
      WaitingForDelivery = warehouseItem.WaitingForDelivery.Value
    };

    return Result<WarehouseItemResponse>.Success(response);
  }
}