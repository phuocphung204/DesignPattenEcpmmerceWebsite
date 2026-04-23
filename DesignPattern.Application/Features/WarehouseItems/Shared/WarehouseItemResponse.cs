
namespace DesignPattern.Application.Features.WarehouseItems.Shared;

public record WarehouseItemResponse
{
  public Guid Id { get; init; }
  public Guid WarehouseId { get; init; }
  public required string WarehouseName { get; init; }
  public Guid ProductId { get; init; }
  public required string ProductName { get; init; }
  public required string Sku { get; init; }
  public required string VariantGroupId { get; init; }
  public int Quantity { get; init; }
  public int WaitingForDelivery { get; init; }
}