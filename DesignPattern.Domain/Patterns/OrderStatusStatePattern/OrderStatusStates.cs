using DesignPattern.Domain.Enums;

namespace DesignPattern.Domain.Patterns.OrderStatusStatePattern;

public sealed class PendingOrderStatusState : OrderStatusStateBase
{
  private static readonly IReadOnlySet<OrderStatusEnum> _allowedTransitions =
    new HashSet<OrderStatusEnum>
    {
      OrderStatusEnum.Processing,
      OrderStatusEnum.Cancelled
    };

  public PendingOrderStatusState() : base(OrderStatusEnum.Pending)
  {
  }

  protected override IReadOnlySet<OrderStatusEnum> AllowedTransitions => _allowedTransitions;
}

public sealed class ProcessingOrderStatusState : OrderStatusStateBase
{
  private static readonly IReadOnlySet<OrderStatusEnum> _allowedTransitions =
    new HashSet<OrderStatusEnum>
    {
      OrderStatusEnum.Shipped
    };

  public ProcessingOrderStatusState() : base(OrderStatusEnum.Processing)
  {
  }

  protected override IReadOnlySet<OrderStatusEnum> AllowedTransitions => _allowedTransitions;
}

public sealed class ShippedOrderStatusState : OrderStatusStateBase
{
  private static readonly IReadOnlySet<OrderStatusEnum> _allowedTransitions =
    new HashSet<OrderStatusEnum>
    {
      OrderStatusEnum.Delivered
    };

  public ShippedOrderStatusState() : base(OrderStatusEnum.Shipped)
  {
  }

  protected override IReadOnlySet<OrderStatusEnum> AllowedTransitions => _allowedTransitions;
}

public sealed class DeliveredOrderStatusState : OrderStatusStateBase
{
  private static readonly IReadOnlySet<OrderStatusEnum> _allowedTransitions =
    new HashSet<OrderStatusEnum>
    {
      OrderStatusEnum.Completed,
      OrderStatusEnum.Returned
    };

  public DeliveredOrderStatusState() : base(OrderStatusEnum.Delivered)
  {
  }

  protected override IReadOnlySet<OrderStatusEnum> AllowedTransitions => _allowedTransitions;
}

public sealed class TerminalOrderStatusState : OrderStatusStateBase
{
  private static readonly IReadOnlySet<OrderStatusEnum> _allowedTransitions = new HashSet<OrderStatusEnum>();

  public TerminalOrderStatusState(OrderStatusEnum status) : base(status)
  {
  }

  protected override IReadOnlySet<OrderStatusEnum> AllowedTransitions => _allowedTransitions;
}

public static class OrderStatusStateFactory
{
  public static IOrderStatusState Create(OrderStatusEnum status)
  {
    return status switch
    {
      OrderStatusEnum.Pending => new PendingOrderStatusState(),
      OrderStatusEnum.Processing => new ProcessingOrderStatusState(),
      OrderStatusEnum.Shipped => new ShippedOrderStatusState(),
      OrderStatusEnum.Delivered => new DeliveredOrderStatusState(),
      OrderStatusEnum.Cancelled => new TerminalOrderStatusState(OrderStatusEnum.Cancelled),
      OrderStatusEnum.Returned => new TerminalOrderStatusState(OrderStatusEnum.Returned),
      OrderStatusEnum.Completed => new TerminalOrderStatusState(OrderStatusEnum.Completed),
      _ => new TerminalOrderStatusState(status)
    };
  }
}