
namespace DesignPattern.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
  IUserRepository UserRepository { get; }
  ICartRepository CartRepository { get; }
  IProductCategoryRepository ProductCategoryRepository { get; }
  IBrandRepository BrandRepository { get; }
  IDiscountCodeRepository DiscountCodeRepository { get; }
  IProductRepository ProductRepository { get; }
  IRatingRepository RatingRepository { get; }
  IWarehouseRepository WarehouseRepository { get; }
  IWarehouseItemRepository WarehouseItemRepository { get; }
  IOrderRepository OrderRepository { get; }
  ISeoRepository SeoRepository { get; }
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}