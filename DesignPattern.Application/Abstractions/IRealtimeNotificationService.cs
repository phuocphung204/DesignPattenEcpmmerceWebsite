namespace DesignPattern.Application.Abstractions;

public interface IRealtimeNotificationService
{
  Task SendToUserAsync(string userId, string message, object data);
  Task SendToGroupAsync(string groupName, string message, object data);
  Task SendToAllAsync(string message, object data);
}
