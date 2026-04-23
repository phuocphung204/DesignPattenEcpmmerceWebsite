using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Repositories;

public interface ICartRepository : IRepository<Cart>, IForTest<Cart>
{
  Task<Cart> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

}
