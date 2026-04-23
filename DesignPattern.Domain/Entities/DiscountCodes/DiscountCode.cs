using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Entities.DiscountCodes;

public abstract class DiscountCode : BaseEntity
{
  public Code Code { get; private set; }
  public Price MinimumOrderAmount { get; private set; }// giá trị đơn hàng tối thiểu để áp dụng mã giảm giá
  public Quantity UsageLimit { get; private set; } // số lần mã giảm giá có thể được sử dụng
  public Quantity UsedCount { get; private set; } = Quantity.Zero; // số lần mã giảm giá đã được sử dụng
  public DateTime ExpirationDate { get; private set; } // ngày hết hạn của mã giảm giá
  public bool IsActive { get; private set; } = true;
  public DiscountType Type { get; private set; }
  public static readonly int MINIMUM_EXPIRATION_TIME = 24; // số giờ tối thiểu từ thời điểm tạo đến ngày hết hạn
  protected DiscountCode(
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    DateTime expirationDate,
    DiscountType type)
  {
    Code = code;
    MinimumOrderAmount = minimumOrderAmount;
    UsageLimit = usageLimit;
    ExpirationDate = expirationDate;
    Type = type;
  }

  protected DiscountCode(
    Guid id,
    DateTime createdAt,
    DateTime? updatedAt,
    Code code,
    Price minimumOrderAmount,
    Quantity usageLimit,
    Quantity usedCount,
    DateTime expirationDate,
    bool isActive,
    DiscountType type)
  {
    Id = id;
    CreatedAt = createdAt;
    UpdatedAt = updatedAt;
    Code = code;
    MinimumOrderAmount = minimumOrderAmount;
    UsageLimit = usageLimit;
    UsedCount = usedCount;
    ExpirationDate = expirationDate;
    IsActive = isActive;
    Type = type;
  }

  //TODO: Template method
  public Result<Price> CheckAppliableDiscount(Price subtotal)
  {
    var activeResult = CheckActive();
    if (activeResult.IsFailure)
      return activeResult.Error;

    var expirationResult = CheckExpiration();
    if (expirationResult.IsFailure)
      return expirationResult.Error;

    var usageResult = CheckUsage();
    if (usageResult.IsFailure)
      return usageResult.Error;

    var minimumResult = CheckMinimum(subtotal);
    if (minimumResult.IsFailure)
      return minimumResult.Error;

    var discount = CalculateDiscount(subtotal); // hook method, sẽ được override ở các class con để tính toán chiết khấu dựa trên loại mã giảm giá

    return discount;
  }

  public Result MarkAsUsed()
  {
    var usageResult = CheckUsage();
    if (usageResult.IsFailure)
      return usageResult.Error;

    UsedCount = UsedCount + Quantity.One;
    Console.WriteLine($"[DiscountCode] MarkAsUsed called. Current UsedCount: {UsedCount.Value}, UsageLimit: {UsageLimit.Value}");
    return true;
  }

  public Result Update(
    DateTime? expirationDate,
    bool? isActive,
    int? usageLimit,
    decimal? minimumOrderAmount = null,
    decimal? fixedAmount = null,
    decimal? percent = null,
    decimal? maximumDiscountAmount = null)
  {
    if (expirationDate is not null)
    {
      var minAllowed = DateTime.UtcNow.AddHours(MINIMUM_EXPIRATION_TIME);
      if (expirationDate.Value < minAllowed)
        return DiscountCodeErrors.InvalidExpirationDate;

      ExpirationDate = expirationDate.Value;
    }

    if (isActive is not null)
      IsActive = isActive.Value;

    if (usageLimit is not null)
    {
      var usageLimitResult = Quantity.Create(usageLimit.Value);
      if (usageLimitResult.IsFailure)
        return usageLimitResult.Error;

      if (usageLimitResult.Value < UsedCount)
        return DiscountCodeErrors.InvalidUsageLimit;

      UsageLimit = usageLimitResult.Value;
    }
    if (minimumOrderAmount is not null)
    {
      var minimumOrderAmountResult = Price.Create(minimumOrderAmount.Value);
      if (minimumOrderAmountResult.IsFailure)
        return minimumOrderAmountResult.Error;

      MinimumOrderAmount = minimumOrderAmountResult.Value;
    }
    var updateResult = updateSpecificProperties(fixedAmount, percent, maximumDiscountAmount);
    if (updateResult.IsFailure)
      return updateResult.Error;

    UpdatedAt = DateTime.UtcNow;
    return true;
  }
  public abstract Result<DiscountCode> updateSpecificProperties(decimal? fixedAmount, decimal? percent, decimal? maximumDiscountAmount);

  public abstract Price CalculateDiscount(Price subtotal); // primitive operation
  private Result CheckActive()
  {
    if (!IsActive)
      return DiscountCodeErrors.Inactive;
    return true;
  }
  private Result CheckExpiration()
  {
    if (DateTime.UtcNow > ExpirationDate)
      return DiscountCodeErrors.InvalidExpirationDate;
    return true;
  }
  private Result CheckUsage()
  {
    if (UsedCount >= UsageLimit)
      return DiscountCodeErrors.UsageLimitReached;
    return true;
  }
  private Result CheckMinimum(Price subtotal)
  {
    if (subtotal < MinimumOrderAmount)
      return DiscountCodeErrors.MinimumOrderAmountNotReached(MinimumOrderAmount);
    return true;
  }

  public virtual Result Update(
    DateTime? expirationDate,
    bool? isActive,
    int? usageLimit
  )
  {
    if (expirationDate is not null)
    {
      if (expirationDate < DateTime.UtcNow.AddHours(MINIMUM_EXPIRATION_TIME))
        return DiscountCodeErrors.InvalidExpirationDate;
      ExpirationDate = (DateTime)expirationDate;
    }

    if (usageLimit is not null)
    {
      var newUsageLimitResult = Quantity.Create((int)usageLimit);
      if (newUsageLimitResult.IsFailure)
        return newUsageLimitResult.Error;

      if (newUsageLimitResult.Value < UsedCount)
        return DiscountCodeErrors.InvalidUsageLimit;
      UsageLimit = newUsageLimitResult.Value;
    }

    if (isActive is not null)
      IsActive = (bool)isActive;

    return Result.Success();
  }
}
