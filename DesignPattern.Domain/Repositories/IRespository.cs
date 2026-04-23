using System.Linq.Expressions;
using DesignPattern.Domain.Common;

namespace DesignPattern.Domain.Repositories;


// 1. Ép kiểu IAggregateRoot để bảo vệ kiến trúc
public interface IRepository<TEntity>
{
  /// <summary>
  /// Lấy danh sách phân trang, có filter, sort và cho phép chọn field rạp sang DTO
  /// </summary>
  /// <typeparam name="TResult">Loại DTO trả về</typeparam> 
  Task<PagedResult<TResult>> GetPagedAsync<TResult>(
    Expression<Func<TEntity, TResult>> selector,                 // Chọn field
    Expression<Func<TEntity, bool>>? predicate = null,           // Lọc (Filter)
    Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null, // Sắp xếp
    int pageIndex = 1,                                           // Trang hiện tại
    int pageSize = 10,
    CancellationToken cancellationToken = default,
    params string[] includeProperties);                          // Join bảng nếu cần

  // ==========================================
  // NHÓM ĐỌC (READ) - Cần Async vì phải I/O với Database
  // ==========================================

  Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  // Trả về IReadOnlyList an toàn hơn IEnumerable trong ngữ cảnh bất đồng bộ
  Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

  // ==========================================
  // NHÓM GHI (WRITE) - KHÔNG Async vì chỉ theo dõi (track) trên RAM
  // ==========================================

  void Create(TEntity entity);

  void Update(TEntity entity);

  void Delete(TEntity entity);
}
