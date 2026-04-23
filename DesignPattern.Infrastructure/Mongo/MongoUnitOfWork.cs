using AutoMapper;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Repositories;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo;

public class MongoUnitOfWork : IUnitOfWork
{
  private readonly MongoContext _context;
  private readonly IMapper _mapper;
  private ICartRepository _cartRepository;
  private IUserRepository _userRepository;
  private IProductCategoryRepository _categoryRepository;
  private IBrandRepository _brandRepository;
  private IDiscountCodeRepository _discountCodeRepository;
  private IProductRepository _productRepository;
  private IRatingRepository _ratingRepository;
  private IWarehouseRepository _warehouseRepository;
  private IWarehouseItemRepository _warehouseItemRepository;
  private IOrderRepository _orderRepository;
  private ISeoRepository _seoRepository;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public MongoUnitOfWork(MongoContext context, IMapper mapper)
  {
    ArgumentNullException.ThrowIfNull(context);
    ArgumentNullException.ThrowIfNull(mapper);
    _context = context;
    _mapper = mapper;
  }

  public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context, _mapper);
  public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_context, _mapper);
  public IProductCategoryRepository ProductCategoryRepository => _categoryRepository ??= new ProductCategoryRepository(_context, _mapper);
  public IBrandRepository BrandRepository => _brandRepository ??= new BrandRepository(_context, _mapper);
  public IDiscountCodeRepository DiscountCodeRepository => _discountCodeRepository ??= new DiscountCodeRepository(_context, _mapper);
  public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_context, _mapper);
  public IRatingRepository RatingRepository => _ratingRepository ??= new RatingRepository(_context, _mapper);
  public IWarehouseRepository WarehouseRepository => _warehouseRepository ??= new WarehouseRepository(_context, _mapper);
  public IWarehouseItemRepository WarehouseItemRepository => _warehouseItemRepository ??= new WarehouseItemRepository(_context, _mapper);
  public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context, _mapper);
  public ISeoRepository SeoRepository => _seoRepository ??= new SeoRepository(_context, _mapper);
  public void Dispose()
  {
    _context?.Dispose();
    //GC.SuppressFinalize(this) gọi để ngăn trình thu rác (GC) gọi finalizer (destructor) của đối tượng này sau khi bạn đã thủ công giải phóng tài nguyên trong Dispose().
    //Nghĩa là: bạn báo với runtime là đã dọn dẹp xong, không cần chạy ~Class() nữa - tránh chi phí gọi finalizer.
    //Nếu lớp không có finalizer thì gọi này vô hại; thường xuất hiện trong pattern Dispose đầy đủ khi có finalizer.
    GC.SuppressFinalize(this);
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}