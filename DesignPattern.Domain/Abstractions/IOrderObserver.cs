using DesignPattern.Domain.Entities.Orders;

namespace DesignPattern.Domain.Abstractions;

public interface IOrderObserver
{
  Task Update(Order order);
}