using DesignPattern.Domain.Enums;
using DiscountCodeDomain = DesignPattern.Domain.Entities.DiscountCodes.DiscountCode;
using DesignPattern.Domain.Entities.DiscountCodes;

namespace DesignPattern.Application.Features.DiscountCodes.Shared;

public record DiscountCodeResponse(
  Guid Id,
  string Code,
  DateTime CreatedAt,
  DateTime ExpirationDate,
  bool IsActive,
  int UsageLimit,
  int UsedCount,
  string Type,
  decimal? Amount,
  decimal? Percent,
  decimal? MaximumDiscountAmount,
  decimal MinimumOrderAmount
)
{
  public static DiscountCodeResponse FromDomain(DiscountCodeDomain discountCode)
  {
    switch (discountCode)
    {
      case FixedDiscountCode fixedDiscount:
        return new DiscountCodeResponse(
          Id: fixedDiscount.Id,
          Code: fixedDiscount.Code.Value,
          CreatedAt: fixedDiscount.CreatedAt,
          ExpirationDate: fixedDiscount.ExpirationDate,
          IsActive: fixedDiscount.IsActive,
          UsageLimit: fixedDiscount.UsageLimit.Value,
          UsedCount: fixedDiscount.UsedCount.Value,
          Type: fixedDiscount.Type.ToString(),
          Amount: fixedDiscount.FixedAmount.Amount,
          Percent: null,
          MaximumDiscountAmount: null,
          MinimumOrderAmount: fixedDiscount.MinimumOrderAmount.Amount
        );
      case PercentageDiscountCode percentageDiscount:
        return new DiscountCodeResponse(
          Id: percentageDiscount.Id,
          Code: percentageDiscount.Code.Value,
          CreatedAt: percentageDiscount.CreatedAt,
          ExpirationDate: percentageDiscount.ExpirationDate,
          IsActive: percentageDiscount.IsActive,
          UsageLimit: percentageDiscount.UsageLimit.Value,
          UsedCount: percentageDiscount.UsedCount.Value,
          Type: percentageDiscount.Type.ToString(),
          Amount: null,
          Percent: percentageDiscount.Percent.Value,
          MaximumDiscountAmount: percentageDiscount.MaximumDiscountAmount.Amount,
          MinimumOrderAmount: percentageDiscount.MinimumOrderAmount.Amount
        );
    }
    throw new InvalidOperationException("Unknown discount code type.");
  }
}
