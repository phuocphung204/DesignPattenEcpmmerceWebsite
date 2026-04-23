using DesignPattern.Domain.Common;
using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Patterns.OrderStatusStatePattern;

public interface IOrderStatusState
{
  OrderStatusEnum Status { get; }

  Result ValidateTransition(OrderStatusEnum newStatus);
}