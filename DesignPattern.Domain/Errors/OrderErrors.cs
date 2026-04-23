namespace DesignPattern.Domain.Errors;

public static class OrderErrors
{
  public static readonly Error ConflictStatusTransition = Error.Conflict("Order.ConflictStatusTransition", "The requested status transition conflicts with the current order status.");
  public static readonly Error InvalidStatusTransition = Error.Validation("Order.InvalidStatusTransition", "Invalid status transition.");
  public static readonly Error InvalidDefaultShippingAddressId = Error.Validation("Order.InvalidDefaultShippingAddressId", "The specified default shipping address ID is invalid.");
  public static readonly Error InsufficientLoyaltyPoints = Error.Validation("Order.InsufficientLoyaltyPoints", "User does not have enough loyalty points to redeem.");
  public static readonly Error InvalidUserId = Error.Validation("Order.InvalidUser", "The specified user is invalid.");
  public static readonly Error OrderNotFound = Error.NotFound("Order.NotFound", "The specified order was not found.");
  public static readonly Error AlreadyPaid = Error.Validation("Order.AlreadyPaid", "The order has already been paid.");
  public static readonly Error UserConflict = Error.Conflict("Order.UserConflict", "The specified user does not have permission to access this order.");
  public static readonly Error EmptyItems = Error.Validation("Order.EmptyItems", "Order must contain at least one item.");
  public static readonly Error InvalidShippingInfo = Error.Validation("Order.InvalidShippingInfo", "Shipping information is invalid.");
  public static readonly Error InvalidPaymentInfo = Error.Validation("Order.InvalidPaymentInfo", "Payment information is invalid.");
  public static readonly Error InsufficientPoints = Error.Validation("Order.InsufficientPoints", "User does not have enough points to use.");
  public static readonly Error InvalidDiscountCode = Error.Validation("Order.InvalidDiscountCode", "The provided discount code is invalid.");
  public static readonly Error InventoryAllocationFailed = Error.Validation("Order.InventoryAllocationFailed", "Failed to allocate inventory for the order.");
  public static readonly Error InvalidOrderPaymentMethod = Error.Validation("Order.InvalidPaymentMethod", "Invalid payment method.");
}