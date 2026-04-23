using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.ValueObjects;

namespace DesignPattern.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>, IForTest<Order>
{
  Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<PagedResult<Order>> SearchListOrderAsync(
    DateTime start,
    DateTime end,
    string? idSearch,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);
  Task<List<Order>> GetListPaidOrdersByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}

public record BuildTimeFilterCriteria
(
  bool annual,
  bool quarterly,
  bool monthly,
  bool weekly,
  DateTime startDate,
  DateTime endDate
);
