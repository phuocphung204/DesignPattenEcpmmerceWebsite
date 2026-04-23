using DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Patterns.SingletonPattern;
using MediatR;

namespace DesignPattern.Application.Features.Orders.Observers;

public class InventoryObserver : IOrderObserver
{
  private static readonly AppLogger _logger = AppLogger.Instance;
  private readonly IMediator _mediator;

  public InventoryObserver(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task Update(Order order)
  {
    _logger.LogInfo($"Event [Inventory] Checking stock/allocation for order {order.Id}.");
    if (order.Status != OrderStatusEnum.Processing)
      return;

    _logger.LogInfo($"Correct order status for inventory check/allocation for order {order.Id}. Status: {order.Status}.");

    var allocationDtos = order.Items
      .Select(item => new ProccessAllocationDTO(
        item.ProductId,
        item.ProductName.Value,
        item.Quantity.Value))
      .ToList();

    var allocationResult = await _mediator.Send(new ProcessAllocationCommand(order.Id, allocationDtos));
    if (allocationResult.IsFailure)
    {
      _logger.LogWarning($"[Inventory] Check stock/allocation failed for order {order.Id}. ErrorCode: {allocationResult.Error.Code}; Message: {allocationResult.Error.Message}");
      return;
    }

    _logger.LogInfo($"[Inventory] Stock check/allocation succeeded for order {order.Id} with {allocationResult.Value.Count} allocations.");
  }
}