using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Entities.DiscountCodes;

public class FixedDiscountCode : DiscountCode
{
  public Price FixedAmount { get; private set; }
  private FixedDiscountCode(
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    DateTime expirationDate,
    Price amount) : base(
      code,
      minimumOrderAmount,
      usageLimit,
      expirationDate,
      DiscountType.FixedAmount)
  {
    FixedAmount = amount;
  }

  private FixedDiscountCode(
    Guid id,
    DateTime createdAt,
    DateTime? updatedAt,
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    Quantity usedCount,
    DateTime expirationDate,
    bool isActive,
    Price amount) : base(
      id,
      createdAt,
      updatedAt,
      code,
      minimumOrderAmount,
      usageLimit,
      usedCount,
      expirationDate,
      isActive,
      DiscountType.FixedAmount)
  {
    FixedAmount = amount;
  }

  private static bool CheckTimeValidity(DateTime expirationDate)
  {
    return DateTime.UtcNow <= expirationDate;
  }

  public static Result<FixedDiscountCode> Create(
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    DateTime expirationDate,
    decimal amount)
  {
    if (!CheckTimeValidity(expirationDate))
    {
      return Error.Validation(
        "ExpirationDate.Invalid",
        "Expiration date must be in the future.");
    }

    var codeResult = Code.Create(code);
    var minimumOrderAmountResult = Price.Create(minimumOrderAmount);
    var usageLimitResult = Quantity.Create(usageLimit);
    var amountResult = Price.Create(amount);

    if (codeResult.IsFailure)
    {
      return codeResult.Error;
    }
    if (minimumOrderAmountResult.IsFailure)
    {
      return minimumOrderAmountResult.Error;
    }
    if (usageLimitResult.IsFailure)
    {
      return usageLimitResult.Error;
    }
    if (amountResult.IsFailure)
    {
      return amountResult.Error;
    }

    return new FixedDiscountCode(
      codeResult.Value,
      minimumOrderAmountResult.Value,
      usageLimitResult.Value,
      expirationDate,
      amountResult.Value);
  }
  public static FixedDiscountCode Rehydrate(
    Guid id,
    DateTime createdAt,
    DateTime? updatedAt,
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    int usedCount,
    DateTime expirationDate,
    bool isActive,
    decimal fixedAmount)
  {
    return new FixedDiscountCode(
      id,
      createdAt,
      updatedAt,
      Code.Rehydrate(code),
      Price.Rehydrate(minimumOrderAmount),
      Quantity.Rehydrate(usageLimit),
      Quantity.Rehydrate(usedCount),
      expirationDate,
      isActive,
      Price.Rehydrate(fixedAmount));
  }
  public override Price CalculateDiscount(Price subTotal)
  {
    return Price.Min(FixedAmount, subTotal);
  }
  public override Result<DiscountCode> updateSpecificProperties(decimal? fixedAmount, decimal? percent, decimal? maximumDiscountAmount)
  {
    if (fixedAmount is not null)
    {
      var fixedAmountResult = Price.Create(fixedAmount.Value);
      if (fixedAmountResult.IsFailure)
      {
        return fixedAmountResult.Error;
      }
      this.FixedAmount = fixedAmountResult.Value;
    }
    return Result<DiscountCode>.Success(this);
  }
}