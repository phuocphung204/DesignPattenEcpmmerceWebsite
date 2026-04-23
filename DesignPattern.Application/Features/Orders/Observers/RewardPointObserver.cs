using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Patterns.SingletonPattern;
using DesignPattern.Domain.Repositories;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Application.Features.Orders.Observers;

public class RewardPointObserver : IOrderObserver
{
  private static readonly AppLogger _logger = AppLogger.Instance;
  private readonly IUnitOfWork _unitOfWork;

  public RewardPointObserver(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task Update(Order order)
  {
    _logger.LogInfo($"Event [RewardPoint] Processing reward points for order {order.Id}.");
    var user = await _unitOfWork.UserRepository.GetByIdAsync(order.UserId);
    if (user is null)
    {
      _logger.LogWarning($"[RewardPoint] Failed to retrieve user {order.UserId} for order {order.Id}. ErrorCode: UserNotFound; Message: User not found");
      return;
    }
    _logger.LogInfo($"[RewardPoint] Processing reward points for user {user.Id} on order {order.Id} with status {order.Status}.");
    if (order.Status == OrderStatusEnum.Processing)
    {
      // Update reward points for the user
      if (order.PointsUsed > Quantity.Zero)
      {
        _logger.LogInfo($"[RewardPoint] Attempting to redeem {order.PointsUsed} points for user {user.Id} on order {order.Id}.");
        var redeemResult = user.RedeemLoyaltyPoints(order.PointsUsed);
        if (redeemResult.IsFailure)
        {
          _logger.LogWarning($"[RewardPoint] Failed to redeem loyalty points for user {order.UserId} on order {order.Id}. ErrorCode: {redeemResult.Error.Code}; Message: {redeemResult.Error.Message}");
          return;
        }
        _logger.LogInfo($"[RewardPoint] Redeemed {order.PointsUsed} points for user {order.UserId} on order {order.Id}.");
      }
    }
    else if (order.Status == OrderStatusEnum.Completed)
    {
      // Add reward points to the user
      user.AddLoyaltyPoints(order.GrandAmount);
      _logger.LogInfo($"[RewardPoint] Added loyalty points for user {order.UserId} on completed order {order.Id}. Grand Total: {order.GrandAmount}.");
    }
    else if (order.Status == OrderStatusEnum.Returned)
    {
      user.DeductLoyaltyPoints(order.GrandAmount);
      _logger.LogInfo($"[RewardPoint] Deducted loyalty points for user {order.UserId} on returned order {order.Id}. Grand Total: {order.GrandAmount}.");
    }

    _logger.LogInfo($"[RewardPoint] Finished processing reward points for user {user.Id} on order {order.Id}. Current points balance: {user.LoyaltyPoints}.");

    _unitOfWork.UserRepository.Update(user);
  }
}