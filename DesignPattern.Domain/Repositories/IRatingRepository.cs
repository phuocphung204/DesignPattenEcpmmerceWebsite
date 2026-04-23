using DesignPattern.Domain.Entities;

namespace DesignPattern.Domain.Repositories;

public interface IRatingRepository : IRepository<Rating>, IForTest<Rating>
{
  Task<List<Rating>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}