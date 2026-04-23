using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Abstractions;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.ValueObjects.Order;
using DesignPattern.Domain.ValueObjects.Payment;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Patterns.OrderStatusStatePattern;
using DesignPattern.Domain.Patterns.ShippingPattern;

namespace DesignPattern.Domain.Entities.Orders;

public class Order : BaseEntity, IOrderSubject
{
  private readonly List<IOrderObserver> _observers = new();
  private readonly Price _pointToMoneyRate = Price.Create(1000m).Value; // 1 điểm = 1000 VNĐ

  public Guid UserId { get; private set; }
  public ShippingInfo ShippingInfo { get; private set; }
  public Code? DiscountCode { get; private set; }
  public Quantity PointsUsed { get; private set; } = Quantity.Zero;
  public Price SubTotal { get; private set; } = Price.Zero; // tổng giá trị trước khi áp dụng giảm giá và phí vận chuyển
  public Price ShippingFee { get; private set; } = Price.Zero;
  public Price DiscountAmount { get; private set; } = Price.Zero;
  public Price RewardPointsAmount { get; private set; } = Price.Zero;
  public Price GrandAmount { get; private set; } = Price.Zero; // tổng giá trị sau khi áp dụng giảm giá và phí vận chuyển
  public PaymentStatusEnum PaymentStatus { get; private set; } = PaymentStatusEnum.Pending;
  public OrderStatusEnum Status { get; private set; } = OrderStatusEnum.Pending;
  public PaymentInfo PaymentInfo { get; private set; }
  public Note? Note { get; private set; }
  public List<OrderItem> Items { get; private set; } = new();
  public List<OrderHistory> Histories { get; private set; } = new();
  public List<InventoryAllocation> InventoryAllocations { get; private set; } = new();

  private Order(
    Guid userId,
    List<OrderItem> items,
    ShippingInfo shippingInfo,
    PaymentInfo paymentInfo,
    Code? discountCode,
    Quantity pointsUsed,

    Note? note)
  {
    UserId = userId;
    Items = items;
    ShippingInfo = shippingInfo;
    PaymentInfo = paymentInfo;
    DiscountCode = discountCode;
    PointsUsed = pointsUsed;
    Note = note;
  }

  public static Result<Order> Create(
    Guid userId,
    List<OrderItem> items,
    ShippingInfo shippingInfo,
    PaymentInfo paymentInfo,
    Code? discountCode,
    Quantity pointsUsed,
    Note? note)
  {
    if (items.Count == 0)
      return Error.Validation("OrderItems.HaveNotItems", "Order must have at least one item.");

    Order order = new Order(
      userId,
      items,
      shippingInfo,
      paymentInfo,
      discountCode,
      pointsUsed,
      note);
    return order;
  }
  public void AddHistory(OrderStatusEnum newStatus, Guid? changedBy, UserRole changedByRole, string note)
  {
    OrderHistory newHistoryResult = OrderHistory.Create(newStatus, changedBy, changedByRole, note);
    Histories.Add(newHistoryResult);
  }

  public Price CalculateSubTotal()
  {
    var subTotal = Items
      .Select(item => item.SellingPrice.Multiply(item.Quantity))
      .Aggregate(Price.Zero, (total, price) => total.Add(price));
    return subTotal;
  }
  public Price CalculateShippingFee()
  {
    var shippingStrategy = ShippingStrategyFactory.Create(ShippingInfo.Type);
    var shippingFee = shippingStrategy.CalculateShippingFee();
    return shippingFee;
  }
  public Price RewardPointsToMoney()
  {
    if (PointsUsed is null)
      return Price.Zero;
    var money = _pointToMoneyRate.Multiply(PointsUsed);
    return money;
  }

  public Price CalculateGrandAmount()
  {
    Price total = SubTotal
    .Add(ShippingFee)
    .Subtract(DiscountAmount)
    .Subtract(RewardPointsToMoney());

    var grandAmount = total.Amount < 0 ? Price.Zero : total;
    return grandAmount;
  }
  public void UpdateCalculationWithDiscount(Price discountAmount)
  {
    DiscountAmount = discountAmount;
    SubTotal = CalculateSubTotal();
    ShippingFee = CalculateShippingFee();
    RewardPointsAmount = RewardPointsToMoney();
    GrandAmount = CalculateGrandAmount();
  }

  public void UpdateCalculationWithoutDiscount()
  {
    SubTotal = CalculateSubTotal();
    ShippingFee = CalculateShippingFee();
    RewardPointsAmount = RewardPointsToMoney();
    GrandAmount = CalculateGrandAmount();
  }

  public void SetOrderStatus(OrderStatusEnum status, string note, Guid? changedBy, UserRole changedByRole)
  {
    Status = status;
    AddHistory(status, changedBy, changedByRole, note);
    // Notify(); thông báo quá sớm Order chưa kịp lưu vào database, khi đó các observer sẽ không lấy được thông tin mới nhất của Order
  }
  public void SetPaymentStatus(PaymentStatusEnum paymentStatus)
  {
    PaymentStatus = paymentStatus;
  }

  public Result TryTransitionStatus(OrderStatusEnum newStatus, string note, Guid? changedBy, UserRole changedByRole)
  {
    var currentState = OrderStatusStateFactory.Create(Status);
    var validationResult = currentState.ValidateTransition(newStatus);
    if (validationResult.IsFailure)
      return validationResult.Error;

    SetOrderStatus(newStatus, note, changedBy, changedByRole);
    return true;
  }

  public Result ConfirmOrder(Guid changedBy)
  {
    if (Status != OrderStatusEnum.Pending)
    {
      return Error.Conflict(
        "Order.Confirm.InvalidStatus",
        "Only pending orders can be confirmed.");
    }
    if (PaymentInfo.Type != PaymentType.Cash && PaymentStatus != PaymentStatusEnum.Paid)
    {
      return Error.Conflict(
        "Order.Confirm.InvalidPaymentStatus",
        "Cannot confirm a pending order with online payment method.");
    }

    return TryTransitionStatus(OrderStatusEnum.Processing, "Order confirmed", changedBy, UserRole.Manager);
  }
  public Result ConfirmPayment(Guid changedBy)
  {
    if (Status != OrderStatusEnum.Processing)
    {
      return Error.Conflict(
        "Order.PaymentConfirmation.InvalidStatus",
        "Only processing orders can be confirmed for payment.");
    }
    if (PaymentInfo.Type == PaymentType.Cash)
    {
      return Error.Conflict(
        "Order.PaymentConfirmation.InvalidPaymentMethod",
        "Cannot confirm payment for cash orders.");
    }
    SetPaymentStatus(PaymentStatusEnum.Paid);
    return true;
  }
  public Result CancelOrderByCustomer(string reasonCancel, Guid? changedBy = null)
  {
    if (Status != OrderStatusEnum.Pending)
      return Error.Conflict("Order.Cancel.InvalidStatus", "Only pending orders can be cancelled.");
    if (PaymentInfo.Type != PaymentType.Cash && PaymentStatus != PaymentStatusEnum.Pending)
      return Error.Conflict("Order.Cancel.InvalidStatus", "Cannot cancel a paid order with online payment method.");
    var noteResult = Note.Create(reasonCancel);
    if (noteResult.IsFailure)
      return noteResult.Error;
    Note = noteResult.Value;

    return TryTransitionStatus(OrderStatusEnum.Cancelled, "Order cancelled by customer", changedBy, UserRole.Customer);
  }
  public Result CancelOrderByManager(string reasonCancel, Guid? changedBy = null)
  {
    if (Status != OrderStatusEnum.Pending)
      return Error.Conflict("Order.Cancel.InvalidStatus", "Only pending orders can be cancelled.");
    if (PaymentInfo.Type != PaymentType.Cash && PaymentStatus != PaymentStatusEnum.Pending)
      return Error.Conflict("Order.Cancel.InvalidStatus", "Cannot cancel a paid order with online payment method.");
    var noteResult = Note.Create(reasonCancel);
    if (noteResult.IsFailure)
      return noteResult.Error;
    Note = noteResult.Value;

    return TryTransitionStatus(OrderStatusEnum.Cancelled, "Order cancelled by manager", changedBy, UserRole.Manager);
  }

  public void SetInventoryAllocation(List<InventoryAllocation> allocations)
  {
    InventoryAllocations = allocations;
  }
  public void Attach(IOrderObserver observer)
  {
    if (!_observers.Contains(observer))
      _observers.Add(observer);
  }

  public void Detach(IOrderObserver observer)
  {
    _observers.Remove(observer);
  }

  public async Task NotifyAsync()
  {
    foreach (var observer in _observers)
      await observer.Update(this);
  }
}
