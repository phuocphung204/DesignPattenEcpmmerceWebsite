using System.Linq.Expressions;
using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.Repositories;

public interface IForTest<TEntity>
{
  Task<PagedResult<TResult>> GetPagedAsyncV2<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    int pageIndex = 1,
    int pageSize = 10,
    bool isDescending = true,
    CancellationToken cancellationToken = default,
    params string[] includeProperties);
}