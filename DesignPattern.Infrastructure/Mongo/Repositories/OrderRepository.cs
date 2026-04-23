using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Orders;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;


namespace DesignPattern.Infrastructure.Mongo.Repositories;

internal class OrderRepository : MongoRepository<Order, OrderDocument>, IOrderRepository
{
  public OrderRepository(MongoContext context, IMapper mapper) : base(context, mapper, "orders") { }

  public override async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<OrderDocument>.Filter.Eq(o => o.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Order>(document);
  }

  public override async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<OrderDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Order>(doc)).ToList();
  }

  public async Task<PagedResult<Order>> SearchListOrderAsync(
    DateTime start,
    DateTime end,
    string? idSearch,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var safePageIndex = pageIndex < 1 ? 1 : pageIndex;
    var safePageSize = pageSize < 1 ? 10 : pageSize;

    var filterBuilder = Builders<OrderDocument>.Filter;
    var filters = new List<FilterDefinition<OrderDocument>>
    {
      filterBuilder.Gte(d => d.CreatedAt, start),
      filterBuilder.Lte(d => d.CreatedAt, end)
    };

    var filter = filterBuilder.And(filters);

    var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

    var documents = await _collection
      .Find(filter)
      .SortByDescending(d => d.CreatedAt)
      .Skip((safePageIndex - 1) * safePageSize)
      .Limit(safePageSize)
      .ToListAsync(cancellationToken);

    if (!string.IsNullOrWhiteSpace(idSearch))
    {
      var keyword = idSearch.Trim();
      documents = documents
        .Where(d => d.Id.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
        .ToList();
    }

    var orders = documents.Select(doc => _mapper.Map<Order>(doc)).ToList();

    return new PagedResult<Order>
    {
      Items = orders,
      TotalCount = (int)totalCount,
      PageIndex = safePageIndex,
      PageSize = safePageSize
    };
  }

  public async Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<OrderDocument>.Filter.Eq(o => o.UserId, userId);
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Order>(doc)).ToList();
  }

  public async Task<List<Order>> GetListPaidOrdersByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
  {
    var filterBuilder = Builders<OrderDocument>.Filter;
    var filter = filterBuilder.And(
      filterBuilder.Eq(d => d.Status, OrderStatusEnum.Completed),
      filterBuilder.Eq(d => d.PaymentStatus, PaymentStatusEnum.Paid),
      filterBuilder.Gte(d => d.CreatedAt, start),
      filterBuilder.Lte(d => d.CreatedAt, end)
    );
    var documents = await _collection.Find(filter).ToListAsync(cancellationToken);
    return documents.Select(d => _mapper.Map<Order>(d)).ToList();
  }

  public override void Create(Order entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<OrderDocument>(entity);
    _context.AddCommand(session => _collection.InsertOneAsync(session, document));
  }
  public override void Update(Order entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<OrderDocument>(entity);
    var updateBuilder = Builders<OrderDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<OrderDocument>>();
    if (document.Status != default)
      updateDefinitions.Add(updateBuilder.Set(d => d.Status, document.Status));

    if (document.PaymentStatus != default)
      updateDefinitions.Add(updateBuilder.Set(d => d.PaymentStatus, document.PaymentStatus));

    if (document.PaymentInfo != null)
      updateDefinitions.Add(updateBuilder.Set(d => d.PaymentInfo, document.PaymentInfo));

    if (document.Histories is { Count: > 0 })
      updateDefinitions.Add(updateBuilder.Set(d => d.Histories, document.Histories));

    if (!string.IsNullOrWhiteSpace(document.Note))
      updateDefinitions.Add(updateBuilder.Set(d => d.Note, document.Note));

    if (document.InventoryAllocations is { Count: > 0 })
      updateDefinitions.Add(updateBuilder.Set(d => d.InventoryAllocations, document.InventoryAllocations));

    updateDefinitions.Add(updateBuilder.Set(d => d.UpdatedAt, DateTime.UtcNow));

    if (updateDefinitions.Count == 0)
      return;

    var updateDefinition = updateBuilder.Combine(updateDefinitions);
    _context.AddCommand(session =>
        _collection.UpdateOneAsync(session,
          d => d.Id == document.Id,
          updateDefinition,
          new UpdateOptions { IsUpsert = false }));
  }
  public override void Delete(Order entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    // var document = _mapper.Map<OrderDocument>(entity);
    // var filter = Builders<OrderDocument>.Filter.Eq(d => d.Id, document.Id);
    // _context.AddCommand(session => _collection.DeleteOneAsync(session, filter));
  }
}