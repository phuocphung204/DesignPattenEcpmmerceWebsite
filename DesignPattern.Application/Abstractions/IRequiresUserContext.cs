namespace DesignPattern.Application.Abstractions;

public interface IRequiresUserContext
{
  // Quyền hạn cần thiết cho AuthorizationBehavior
  Guid UserId { get; set; }
}
