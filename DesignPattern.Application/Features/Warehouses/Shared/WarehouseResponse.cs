
namespace DesignPattern.Application.Features.Warehouses.Shared;

public sealed record WarehouseResponse
{
  public Guid Id { get; init; }
  public string Name { get; init; }
  public string Address { get; init; }
}