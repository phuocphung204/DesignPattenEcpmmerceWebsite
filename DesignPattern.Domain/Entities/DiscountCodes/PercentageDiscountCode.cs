using DesignPattern.Domain.Common;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Entities.DiscountCodes;

public class PercentageDiscountCode : DiscountCode
{
  public Percent Percent { get; private set; }
  public Price MaximumDiscountAmount { get; private set; }

  private PercentageDiscountCode(
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    DateTime expirationDate,
    Percent percent,
    Price maximumDiscountAmount) : base(
      code,
      minimumOrderAmount,
      usageLimit,
      expirationDate,
      DiscountType.Percentage)
  {
    Percent = percent;
    MaximumDiscountAmount = maximumDiscountAmount;
  }

  private PercentageDiscountCode(
    Guid id,
    DateTime createdAt,
    DateTime? updatedAt,
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    Quantity usedCount,
    DateTime expirationDate,
    bool isActive,
    Percent percent,
    Price maximumDiscountAmount) : base(
      id,
      createdAt,
      updatedAt,
      code,
      minimumOrderAmount,
      usageLimit,
      usedCount,
      expirationDate,
      isActive,
      DiscountType.Percentage)
  {
    Percent = percent;
    MaximumDiscountAmount = maximumDiscountAmount;
  }

  private static bool CheckTimeValidity(DateTime expirationDate)
  {
    return DateTime.UtcNow <= expirationDate;
  }

  private static bool CheckMaximumDiscountAmount(Price maximumDiscountAmount, Percent percent, Price minimumOrderAmount)
  {
    // MaximumDiscountAmount < MinimumOrderAmount * Percent
    return maximumDiscountAmount < percent.ApplyToPrice(minimumOrderAmount);
  }

  public static Result<PercentageDiscountCode> Create(
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    DateTime expirationDate,
    decimal percent,
    decimal maximumDiscountAmount)
  {

    var codeResult = Code.Create(code);
    var minimumOrderAmountResult = Price.Create(minimumOrderAmount);
    var usageLimitResult = Quantity.Create(usageLimit);
    var percentResult = Percent.Create(percent);
    var maximumDiscountAmountResult = Price.Create(maximumDiscountAmount);

    if (!CheckTimeValidity(expirationDate))
    {
      return Error.Validation(
        "ExpirationDate.Invalid",
        "Expiration date must be in the future.");
    }

    if (!CheckMaximumDiscountAmount(maximumDiscountAmountResult.Value, percentResult.Value, minimumOrderAmountResult.Value))
    {
      return Error.Validation(
        "MaximumDiscountAmount.Invalid",
        "Maximum discount amount must be less than minimum order amount multiplied by percent.");
    }

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
    if (percentResult.IsFailure)
    {
      return percentResult.Error;
    }
    if (maximumDiscountAmountResult.IsFailure)
    {
      return maximumDiscountAmountResult.Error;
    }

    return new PercentageDiscountCode(
      codeResult.Value,
      minimumOrderAmountResult.Value,
      usageLimitResult.Value,
      expirationDate,
      percentResult.Value,
      maximumDiscountAmountResult.Value);
  }

  public static PercentageDiscountCode Rehydrate(
    Guid id,
    DateTime createdAt,
    DateTime? updatedAt,
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    int usedCount,
    DateTime expirationDate,
    bool isActive,
    decimal percent,
    decimal maximumDiscountAmount)
  {
    return new PercentageDiscountCode(
      id,
      createdAt,
      updatedAt,
      Code.Rehydrate(code),
      Price.Rehydrate(minimumOrderAmount),
      Quantity.Rehydrate(usageLimit),
      Quantity.Rehydrate(usedCount),
      expirationDate,
      isActive,
      Percent.Rehydrate(percent),
      Price.Rehydrate(maximumDiscountAmount));
  }
  public override Price CalculateDiscount(Price subTotal)
  {
    Price discountAmount = Percent.ApplyToPrice(subTotal);
    return Price.Min(discountAmount, MaximumDiscountAmount);
  }
  public override Result<DiscountCode> updateSpecificProperties(decimal? fixedAmount, decimal? percent, decimal? maximumDiscountAmount)
  {
    if (percent is not null)
    {
      var percentResult = Percent.Create(percent.Value);
      if (percentResult.IsFailure)
      {
        return percentResult.Error;
      }
      Percent = percentResult.Value;
    }

    if (maximumDiscountAmount is not null)
    {
      var maximumDiscountAmountResult = Price.Create(maximumDiscountAmount.Value);
      if (maximumDiscountAmountResult.IsFailure)
      {
        return maximumDiscountAmountResult.Error;
      }
      if (!CheckMaximumDiscountAmount(maximumDiscountAmountResult.Value, Percent, MinimumOrderAmount))
      {
        return Error.Validation(
          "MaximumDiscountAmount.Invalid",
          "Maximum discount amount must be less than minimum order amount multiplied by percent.");
      }
      MaximumDiscountAmount = maximumDiscountAmountResult.Value;
    }

    return this;
  }

}
