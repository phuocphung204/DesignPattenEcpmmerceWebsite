using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.Repositories;

public interface ISeoRepository : IRepository<Seo>, IForTest<Seo>
{
  Task<Seo> GetByReferenceIdAsync(Guid productId, CancellationToken cancellationToken = default);
}