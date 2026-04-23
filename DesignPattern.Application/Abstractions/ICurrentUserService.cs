using DesignPattern.Domain.Enums;

namespace DesignPattern.Application.Abstractions;

public interface ICurrentUserService
{
  // Trả về UserId của người đang login (dạng string hoặc Guid)
  Guid UserId { get; }

  // Kiểm tra xem đã đăng nhập chưa
  bool IsAuthenticated { get; }

  // Có thể thêm Email hoặc Roles nếu cần
  string? Email { get; }

  UserRole Role { get; }
}
