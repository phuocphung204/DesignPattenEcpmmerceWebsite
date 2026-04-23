using DesignPattern.Domain.ValueObjects;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Errors;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;

namespace DesignPattern.Domain.ValueObjects.Order;

public sealed record class OrderHistory
{
  public OrderStatusEnum Status { get; init; }
  public DateTime StatusChangedDate { get; init; } = DateTime.UtcNow;
  public Guid? ChangedBy { get; init; }
  public UserRole ChangedByRole { get; init; }
  public string Note { get; init; }

  private OrderHistory(OrderStatusEnum status, Guid? changedBy, UserRole changedByRole, string note)
  {
    Status = status;
    ChangedBy = changedBy;
    ChangedByRole = changedByRole;
    Note = note;
  }

  public static OrderHistory Create(OrderStatusEnum status, Guid? changedBy, UserRole changedByRole, string note)
  {
    OrderHistory orderHistory = new OrderHistory(status, changedBy, changedByRole, note);
    return orderHistory;
  }
}