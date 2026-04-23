using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Entities.DiscountCodes;

public abstract class DiscountData
{
  public string Code { get; }
  public decimal MinimumOrderAmount { get; }
  public int UsageLimit { get; }
  public DateTime ExpirationDate { get; }

  protected DiscountData(
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    DateTime expirationDate)
  {
    Code = code;
    MinimumOrderAmount = minimumOrderAmount;
    UsageLimit = usageLimit;
    ExpirationDate = expirationDate;
  }
}

public class PercentageDiscountData : DiscountData
{
  public decimal Percent { get; }
  public decimal MaximumDiscountAmount { get; }

  public PercentageDiscountData(
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    DateTime expirationDate,
    decimal percent,
    decimal maximumDiscountAmount)
    : base(code, minimumOrderAmount, usageLimit, expirationDate)
  {
    Percent = percent;
    MaximumDiscountAmount = maximumDiscountAmount;
  }
}

public class FixedAmountDiscountData : DiscountData
{
  public decimal FixedAmount { get; }

  public FixedAmountDiscountData(
    string code,
    decimal minimumOrderAmount,
    int usageLimit,
    DateTime expirationDate,
    decimal fixedAmount)
    : base(code, minimumOrderAmount, usageLimit, expirationDate)
  {
    FixedAmount = fixedAmount;
  }
}