using MediatR;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects.Order;

namespace DesignPattern.Application.Features.WarehouseItems.Commands.ProccessAllocation;

public record ProccessAllocationDTO
(
  Guid ProductId,
  string ProductName,
  int RequestedQuantity
);

public record ProcessAllocationCommand(Guid orderId, List<ProccessAllocationDTO> dtos) : IRequest<Result<List<InventoryAllocation>>>
{
}
