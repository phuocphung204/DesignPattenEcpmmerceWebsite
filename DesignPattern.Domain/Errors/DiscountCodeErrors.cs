using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Errors;

public static class DiscountCodeErrors
{
  public static readonly Error InvalidExpirationDate = Error.Validation(
    "DiscountCode.InvalidExpirationDate",
    $"Expiration date must be at least {DiscountCode.MINIMUM_EXPIRATION_TIME} hours after creation date.");

  public static readonly Error NotFound = Error.NotFound(
    "DiscountCode.NotFound",
    "The discount code was not found.");

  public static readonly Error Inactive = Error.Validation(
    "DiscountCode.Inactive",
    "The discount code is not active.");

  public static Error MinimumOrderAmountNotReached(Price minimumOrderAmount) => Error.Validation(
    "DiscountCode.MinimumOrderAmountNotReached",
    $"The order amount must be at least {minimumOrderAmount.Amount} to apply this discount code.");

  public static readonly Error InvalidDiscountType = Error.Validation(
    "DiscountCode.InvalidDiscountType",
    "The discount type is invalid.");

  public static readonly Error UsageLimitReached = Error.Validation(
    "DiscountCode.UsageLimitReached",
    "The discount code has reached its usage limit.");

  public static readonly Error InvalidUsageLimit = Error.Validation(
    "DiscountCode.InvalidUsageLimit",
    "Usage limit must be greater than or equal to used count.");
}
