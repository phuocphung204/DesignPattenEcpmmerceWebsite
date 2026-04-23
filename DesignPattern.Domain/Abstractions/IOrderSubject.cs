namespace DesignPattern.Domain.Abstractions;

public interface IOrderSubject
{
  void Attach(IOrderObserver observer);

  void Detach(IOrderObserver observer);

  Task NotifyAsync();
}