namespace DesignPattern.Domain.Errors;

public static class PaymentErrors
{
  public static readonly Error InvalidPaymentMethod = Error.Conflict("Payment.InvalidPaymentMethod", "The specified payment method is invalid.");
  public static readonly Error NotFound = Error.NotFound("Payment.NotFound", "The specified payment was not found.");
  public static readonly Error PaymentFailed = Error.Validation("Payment.PaymentFailed", "The payment failed.");
}
