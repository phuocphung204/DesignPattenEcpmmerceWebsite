using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects.Order;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;

public class ProcessAllocationCommandHandler : IRequestHandler<ProcessAllocationCommand, Result<List<InventoryAllocation>>>
{
  private readonly IUnitOfWork _unitOfWork;

  public ProcessAllocationCommandHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task<Result> CheckStock(List<ProccessAllocationDTO> dtos, CancellationToken cancellationToken)
  {
    foreach (var dto in dtos)
    {
      var warehouseItems = await _unitOfWork.WarehouseItemRepository
        .GetListWarehouseItemByProductIdAsync(dto.ProductId, cancellationToken);

      if (warehouseItems is null || warehouseItems.Count == 0)
        return WarehouseErrors.CheckWarehouseItemNotFound(dto.ProductId);

      var Stock = warehouseItems.Sum(item => item.GetStock().Value);
      if (Stock < dto.RequestedQuantity)
        return WarehouseErrors.InsufficientStock(dto.ProductId, dto.RequestedQuantity, Stock);
    }
    return true;
  }
  public async Task<Result<List<InventoryAllocation>>> Handle(ProcessAllocationCommand request, CancellationToken cancellationToken)
  {
    var dtos = request.dtos;

    // Kiểm tra tồn kho cho tất cả các sản phẩm trên đơn hàng trước khi tiến hành phân bổ
    var checkStockResult = await CheckStock(dtos, cancellationToken);
    if (checkStockResult.IsFailure)
      return checkStockResult.Error;

    // Danh sách để lưu trữ kết quả phân bổ hàng tồn kho cho từng sản phẩm
    List<InventoryAllocation> responseList = new();

    foreach (var dto in dtos)
    {
      var warehouseItems = await _unitOfWork.WarehouseItemRepository
        .GetListWarehouseItemByProductIdAsync(dto.ProductId, cancellationToken);

      if (warehouseItems.Count == 0)
        return WarehouseErrors.CheckWarehouseItemNotFound(dto.ProductId);

      // Tiến hành phân bổ hàng tồn kho từ các kho cho đến khi đủ số lượng yêu cầu
      var remainingResult = Quantity.Create(dto.RequestedQuantity);
      if (remainingResult.IsFailure)
        return remainingResult.Error;

      var remaining = remainingResult.Value;
      var Allsolation = new List<(InventoryAllocation Allocation, Quantity Remaining)>();

      foreach (var item in warehouseItems)
      {
        var result = item.AddWarehouseItemWaitingForDelivery(remaining);
        Allsolation.Add(result);
        remaining = result.Remaining;

        _unitOfWork.WarehouseItemRepository.Update(item);

        // Nếu đã phân bổ đủ số lượng yêu cầu thì dừng lại
        if (remaining == Quantity.Zero)
          break;
      }
      foreach (var allocation in Allsolation)
      {
        var response = InventoryAllocation.Create
        (
          allocation.Allocation.WarehouseId,
          allocation.Allocation.WarehouseName,
          allocation.Allocation.ProductId,
          allocation.Allocation.ProductName,
          allocation.Allocation.Allocated
        );
        responseList.Add(response);
      }
    }

    // Cập nhật thông tin phân bổ hàng tồn kho vào đơn hàng
    var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.orderId, cancellationToken);
    if (order is null)
      return OrderErrors.OrderNotFound;
    order.SetInventoryAllocation(responseList);
    _unitOfWork.OrderRepository.Update(order);

    // Lưu thay đổi vào cơ sở dữ liệu
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return responseList;
  }
}