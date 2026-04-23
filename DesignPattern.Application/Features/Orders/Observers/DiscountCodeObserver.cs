using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Patterns.SingletonPattern;
using DesignPattern.Domain.Repositories;

namespace DesignPattern.Application.Features.Orders.Observers;

public class DiscountCodeObserver : IOrderObserver
{
  private static readonly AppLogger _logger = AppLogger.Instance;
  private readonly IUnitOfWork _unitOfWork;

  public DiscountCodeObserver(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task Update(Order order)
  {
    _logger.LogInfo($"Event [DiscountCode] Checking discount code for order {order.Id}.");
    if (order.Status != OrderStatusEnum.Processing)
      return;

    _logger.LogInfo($"Correct order status for discount code processing for order {order.Id}. Status: {order.Status}.");

    if (order.DiscountCode is null)
    {
      _logger.LogInfo($"[DiscountCode] No discount code applied for order {order.Id}.");
      return;
    }

    var discountCode = await _unitOfWork.DiscountCodeRepository.GetByCodeAsync(order.DiscountCode.Value);
    if (discountCode is null)
    {
      _logger.LogWarning($"[DiscountCode] Failed to retrieve discount code {order.DiscountCode.Value} for order {order.Id}. ErrorCode: DiscountCodeNotFound; Message: Discount code not found");
      return;
    }

    // Mark the discount code as used
    var markAsUsedResult = discountCode.MarkAsUsed();
    _logger.LogInfo($"[DiscountCode] Marking discount code {discountCode.Code.Value} as used for order {order.Id}. Result: {markAsUsedResult.IsSuccess}");
    if (markAsUsedResult.IsFailure)
    {
      _logger.LogWarning($"[DiscountCode] Failed to mark discount code {order.DiscountCode.Value} as used for order {order.Id}. ErrorCode: {markAsUsedResult.Error.Code}; Message: {markAsUsedResult.Error.Message}");
      return;
    }

    _unitOfWork.DiscountCodeRepository.Update(discountCode);

    _logger.LogInfo($"[DiscountCode] Discount code {order.DiscountCode.Value} marked as used for order {order.Id}.");
  }
}