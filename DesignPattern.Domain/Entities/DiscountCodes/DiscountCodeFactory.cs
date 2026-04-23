using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Entities.DiscountCodes;

public static class DiscountCodeFactory
{
  public static Result<DiscountCode> Create(DiscountData data)
  {
    switch (data)
    {
      case PercentageDiscountData percentageData:
        var percentageDiscountCode = PercentageDiscountCode.Create(
          percentageData.Code,
          percentageData.MinimumOrderAmount,
          percentageData.UsageLimit,
          percentageData.ExpirationDate,
          percentageData.Percent,
          percentageData.MaximumDiscountAmount);
        if (percentageDiscountCode.IsFailure)
        {
          return Result<DiscountCode>.Failure(percentageDiscountCode.Error);
        }
        return Result<DiscountCode>.Success(percentageDiscountCode.Value);

      case FixedAmountDiscountData fixedAmountData:
        var fixedAmountDiscountCode = FixedDiscountCode.Create(
          fixedAmountData.Code,
          fixedAmountData.MinimumOrderAmount,
          fixedAmountData.UsageLimit,
          fixedAmountData.ExpirationDate,
          fixedAmountData.FixedAmount);
        if (fixedAmountDiscountCode.IsFailure)
        {
          return Result<DiscountCode>.Failure(fixedAmountDiscountCode.Error);
        }
        return Result<DiscountCode>.Success(fixedAmountDiscountCode.Value);
      default:
        return Result<DiscountCode>.Failure(new Error(
          "DiscountType.Invalid",
          "Invalid discount type."));
    }
  }
}