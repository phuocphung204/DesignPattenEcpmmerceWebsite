using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;

namespace DesignPattern.Domain.Patterns.OrderStatusStatePattern;

public abstract class OrderStatusStateBase : IOrderStatusState
{
  protected OrderStatusStateBase(OrderStatusEnum status)
  {
    Status = status;
  }

  public OrderStatusEnum Status { get; }

  protected abstract IReadOnlySet<OrderStatusEnum> AllowedTransitions { get; }

  public Result ValidateTransition(OrderStatusEnum newStatus)
  {
    if (newStatus == Status)
      return OrderErrors.ConflictStatusTransition;

    return AllowedTransitions.Contains(newStatus)
      ? Result.Success()
      : OrderErrors.InvalidStatusTransition;
  }
}