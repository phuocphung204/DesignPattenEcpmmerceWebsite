
namespace DesignPattern.Domain.Errors;

public static class WarehouseErrors
{
  public static readonly Error WarehouseNotFound = Error.NotFound(
      "Warehouse.NotFound",
      "The warehouse with the specified identifier was not found.");
  public static readonly Error WarehouseItemNotFound = Error.NotFound(
      "WarehouseItem.NotFound",
      "The warehouse item with the specified identifier was not found.");
  public static Error CheckWarehouseItemNotFound(Guid id) => Error.NotFound(
      "WarehouseItem.NotFound",
      $"The product with the specified ID '{id}' was not found in the warehouse.");

  public static Error InsufficientStock(Guid id, int requestedQuantity, int availableQuantity) => Error.Failure(
      "WarehouseItem.InsufficientStock",
      $"The product with ID '{id}' does not have sufficient stock to fulfill the allocation request. Requested: {requestedQuantity}, Available: {availableQuantity}");

  public static readonly Error DuplicateProductId = Error.Failure(
      "WarehouseItem.DuplicateProductId",
      "The product already exists in the warehouse.");
  public static readonly Error QuantityLessThanWaitingForDelivery = Error.Failure(
      "WarehouseItem.QuantityLessThanWaitingForDelivery",
      "The updated quantity cannot be less than the quantity waiting for delivery.");
}