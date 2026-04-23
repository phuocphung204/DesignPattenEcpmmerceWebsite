using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Abstractions;

public interface IAuthorizeableRequest // có thể ủy quyền
{
  public UserRole[] Roles { get; }
}
