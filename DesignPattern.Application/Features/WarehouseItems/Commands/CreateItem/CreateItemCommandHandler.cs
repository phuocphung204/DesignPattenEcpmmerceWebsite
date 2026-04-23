using MediatR;
using DesignPattern.Application.Features.WarehouseItems.Shared;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Result<WarehouseItemResponse>>
{
  private readonly IUnitOfWork _unitOfWork;

  public CreateItemCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<WarehouseItemResponse>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
  {
    var dto = request.dto;
    var warehouseItemResult = WarehouseItem.Create(
      dto.WarehouseId,
      dto.WarehouseName,
      dto.ProductId,
      dto.ProductName,
      dto.Sku,
      dto.VariantGroupId,
      dto.Quantity);
    if (warehouseItemResult.IsFailure)
      return Result<WarehouseItemResponse>.Failure(warehouseItemResult.Error);

    var warehouseItem = warehouseItemResult.Value;

    _unitOfWork.WarehouseItemRepository.Create(warehouseItem);
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