using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.DiscountCodes;

namespace DesignPattern.Domain.Repositories;

public interface IDiscountCodeRepository : IRepository<DiscountCode>, IForTest<DiscountCode>
{
  Task<PagedResult<DiscountCodeReadModel>> SearchDiscountCodesPagedAsync(
    string? searchTerm = null,
    bool? isActive = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);

  Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public sealed record DiscountCodeReadModel
{
  public Guid Id { get; init; }
  public string Code { get; init; } = string.Empty;
  public DateTime CreatedAt { get; init; }
  public DateTime ExpirationDate { get; init; }
  public bool IsActive { get; init; }
  public int UsageLimit { get; init; }
  public int UsedCount { get; init; }
  public decimal MinimumOrderAmount { get; init; }
  public decimal? FixedAmount { get; init; }
  public decimal? Percent { get; init; }
  public decimal? MaximumDiscountAmount { get; init; }
}
