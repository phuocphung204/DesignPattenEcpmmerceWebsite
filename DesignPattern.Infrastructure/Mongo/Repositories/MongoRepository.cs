using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

public abstract class MongoRepository<TEntity, TDocument> : IRepository<TEntity>
    where TEntity : class
    where TDocument : class
{
  protected readonly IMongoCollection<TDocument> _collection;
  protected readonly MongoContext _context;
  protected readonly IMongoCollection<ProductCategoryDocument> _productCategoryCollection;
  protected readonly IMapper _mapper; // AutoMapper

  public MongoRepository(MongoContext context, IMapper mapper, string collectionName)
  {
    _context = context;
    _collection = _context.GetCollection<TDocument>(collectionName);
    _productCategoryCollection = context.GetCollection<ProductCategoryDocument>(collectionName);
    _mapper = mapper;
  }

  public abstract Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

  public abstract Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  public async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? orderBy = null,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default,
    params string[] includeProperties)
  {
    var query = _collection.AsQueryable();

    // Chuyển đổi selector từ Entity sang Document
    var docSelector = _mapper.MapExpression<Expression<Func<TDocument, TResult>>>(selector);
    // Chuyển đổi predicate từ Entity sang Document
    var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<TDocument, bool>>>(predicate) : null;
    var docOrderBy = orderBy != null ? _mapper.MapExpression<Expression<Func<IQueryable<TDocument>, IOrderedQueryable<TDocument>>>>(orderBy).Compile() : null;

    if (orderBy != null)
    {
      Console.WriteLine(orderBy);
    }

    if (docPredicate != null) query = query.Where(docPredicate);

    // Lấy tổng số document
    var totalCount = await query.CountAsync(cancellationToken);

    IQueryable<TResult> queryWithOrder;
    if (docOrderBy != null)
    {
      // Lấy danh sách document đã phân trang
      queryWithOrder = docOrderBy(query)
      .Select(docSelector)
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize);
    }
    else
    {
      queryWithOrder = query
      .Select(docSelector)
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize);
    }

    var finalQuery = queryWithOrder;
    Console.WriteLine(finalQuery);
    // Chuyển đổi danh sách document sang TResult
    var items = await finalQuery.ToListAsync(cancellationToken);

    return new PagedResult<TResult>
    {
      Items = items,
      TotalCount = (int)totalCount,
      PageIndex = pageIndex,
      PageSize = pageSize
    };
  }

  public async Task<PagedResult<TResult>> GetPagedAsyncV2<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    int pageIndex = 1,
    int pageSize = 10,
    bool isDescending = true,
    CancellationToken cancellationToken = default,
    params string[] includeProperties)
  {
    // 1. Chuyển đổi Filter từ Entity sang Document (Vẫn lọc ở DB nếu có thể)

    var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<TDocument, bool>>>(predicate) : null;
    Expression<Func<ProductCategory, bool>> myEntityPredicate = (entity) => entity.Name.Value.ToLowerInvariant().Contains("mini");
    // var temp = myEntityPredicate.Compile(); // This is no longer needed as we pass the expression directly
    // Expression<Func<ProductCategoryDocument, bool>> myDocPredicate = (doc) => doc.Name.Contains("Laptop");
    Expression<Func<ProductCategoryDocument, bool>> myDocPredicate = _mapper.MapExpression<Expression<Func<ProductCategoryDocument, bool>>>(myEntityPredicate);
    var docSelector = _mapper.MapExpression<Expression<Func<TDocument, TResult>>>(selector);

    var query = _collection.AsQueryable();
    var myQuery = _productCategoryCollection.AsQueryable().Where(myDocPredicate);
    Console.WriteLine(myDocPredicate);

    if (docPredicate != null)
    {
      query = query.Where(docPredicate);
    }

    var totalCount = await query.CountAsync(cancellationToken);

    // Apply paging and projection.
    // We execute the query directly into a List<TResult> to avoid IQueryable type mismatch.
    var items = await query
      .Skip((pageIndex - 1) * pageSize)
      .Take(pageSize)
      .Select(docSelector)
      .ToListAsync(cancellationToken);

    return new PagedResult<TResult>
    {
      Items = items,
      TotalCount = (int)totalCount,
      PageIndex = pageIndex,
      PageSize = pageSize
    };
  }

  private static void PrintQuery(string query)
  {
    Console.WriteLine(query);
  }

  public abstract void Create(TEntity entity);

  public abstract void Delete(TEntity entity);
  public abstract void Update(TEntity entity);
}
