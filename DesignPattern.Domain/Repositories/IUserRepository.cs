using System.Linq.Expressions;
using DesignPattern.Domain.Entities.Users;

namespace DesignPattern.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
  Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);

  Task<User> GetUserWithSelectorAsync<TSelector>(
      Guid? userId,
      string? email,
      Expression<Func<User, TSelector>>? selector,
      CancellationToken cancellationToken = default);

  Task<User> GetAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken);

  void UpdateAddresses(User entity);

  /// <summary>
  /// Lấy các thông tin cơ bản của user (id, email, passwordHash, role, status, fullName, avatarLink)
  /// </summary>
  /// <param name="email"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  Task<User> GetAuthInfoByUserIdAsync(Guid userId, CancellationToken cancellationToken);

  Task<User> GetAuthInfoByEmailAsync(string email, CancellationToken cancellationToken);

  Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken);

  // Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken);

  /// <summary>
  /// Kiểm tra số điện thoại đã tồn tại hay chưa, loại trừ user hiện tại (dùng cho cập nhật profile)
  /// </summary>
  // Task<bool> IsPhoneExistExcludingUserAsync(string phoneNumber, Guid excludeUserId, CancellationToken cancellationToken);
}
