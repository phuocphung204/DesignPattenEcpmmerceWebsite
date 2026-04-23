using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.Inventory;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Text.RegularExpressions;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

internal class WarehouseItemRepository : MongoRepository<WarehouseItem, WarehouseItemDocument>, IWarehouseItemRepository
{
  public WarehouseItemRepository(MongoContext context, IMapper mapper) : base(context, mapper, "warehouseItems") { }

  public override async Task<WarehouseItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseItemDocument>.Filter.Eq(c => c.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<WarehouseItem>(document);
  }
  // search nhiều tiêu chí chuẩn string nhưng trả về List<WarehouseItem> đầy đủ thông tin warehouse item
  public async Task<List<WarehouseItem>> SearchAsync(WarehouseItemSearchCriteria criteria, CancellationToken cancellationToken = default)
  {
    var filterBuilder = Builders<WarehouseItemDocument>.Filter;
    var filterDefinitions = new List<FilterDefinition<WarehouseItemDocument>>();

    if (criteria.WarehouseId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.WarehouseId, criteria.WarehouseId.Value));
    }

    if (!string.IsNullOrWhiteSpace(criteria.ProductName))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.ProductName, criteria.ProductName));
    }

    if (!string.IsNullOrWhiteSpace(criteria.Sku))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.Sku, criteria.Sku));
    }

    if (!string.IsNullOrWhiteSpace(criteria.VariantGroupId))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.VariantGroupId, criteria.VariantGroupId));
    }

    if (criteria.ProductId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.ProductId, criteria.ProductId.Value));
    }

    var filter = filterDefinitions.Count > 0
      ? filterBuilder.And(filterDefinitions)
      : filterBuilder.Empty;

    var documents = await _collection
      .Find(filter)
      .ToListAsync(cancellationToken);

    return documents.Select(doc => _mapper.Map<WarehouseItem>(doc)).ToList();
  }

  public async Task<List<WarehouseItem>> GetListWarehouseItemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseItemDocument>.Filter.Eq(c => c.ProductId, productId);
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<WarehouseItem>(doc)).ToList();
  }
  // search nhiều tiêu chí gần đúng Name được nhưng trả về List<WarehouseItem> chỉ có một số thông tin cần thiết để hiển thị danh sách warehouse item
  public async Task<PagedResult<WarehouseItem>> GetPagedAsync(
    WarehouseItemSearchCriteria criteria,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var safePageIndex = pageIndex < 1 ? 1 : pageIndex;
    var safePageSize = pageSize < 1 ? 10 : pageSize;

    var filterBuilder = Builders<WarehouseItemDocument>.Filter;
    var filterDefinitions = new List<FilterDefinition<WarehouseItemDocument>>();

    if (criteria.WarehouseId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.WarehouseId, criteria.WarehouseId.Value));
    }

    if (!string.IsNullOrWhiteSpace(criteria.ProductName))
    {
      var escapedName = Regex.Escape(criteria.ProductName.Trim());
      var namePattern = $".*{escapedName}.*";
      filterDefinitions.Add(filterBuilder.Regex(
        x => x.ProductName,
        new BsonRegularExpression(namePattern, "i")));
    }

    if (!string.IsNullOrWhiteSpace(criteria.Sku))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.Sku, criteria.Sku));
    }

    if (!string.IsNullOrWhiteSpace(criteria.VariantGroupId))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.VariantGroupId, criteria.VariantGroupId));
    }

    if (criteria.ProductId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.ProductId, criteria.ProductId.Value));
    }

    var filter = filterDefinitions.Count > 0
      ? filterBuilder.And(filterDefinitions)
      : filterBuilder.Empty;

    var totalCount = await _collection.CountDocumentsAsync(
      filter,
      cancellationToken: cancellationToken);

    var documents = await _collection
      .Find(filter)
      .Skip((safePageIndex - 1) * safePageSize)
      .Limit(safePageSize)
      .ToListAsync(cancellationToken);

    var items = documents
      .Select(doc => _mapper.Map<WarehouseItem>(doc))
      .ToList();

    return new PagedResult<WarehouseItem>
    {
      Items = items,
      TotalCount = (int)totalCount,
      PageIndex = safePageIndex,
      PageSize = safePageSize
    };
  }

  public override async Task<List<WarehouseItem>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseItemDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<WarehouseItem>(doc)).ToList();
  }

  public async Task<bool> ExistsByProductIdAsync(Guid warehouseId, Guid productId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseItemDocument>.Filter;
    var exists = await _collection.Find(filter.And(
      filter.Eq(d => d.WarehouseId, warehouseId),
      filter.Eq(d => d.ProductId, productId)
    )).AnyAsync(cancellationToken);
    return exists;
  }

  public override void Create(WarehouseItem entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<WarehouseItemDocument>(entity);
    _context.AddCommand(async session =>
    {
      var filterBuilder = Builders<WarehouseItemDocument>.Filter;
      var exists = await _collection.Find(session, filterBuilder.And(
        filterBuilder.Eq(d => d.WarehouseId, entity.WarehouseId),
        filterBuilder.Eq(d => d.ProductId, entity.ProductId)
      )).AnyAsync();

      if (exists)
      {
        throw new InvalidOperationException($"Warehouse item with product ID {entity.ProductId} already exists in warehouse {entity.WarehouseId}");
      }

      await _collection.InsertOneAsync(session, document);
    });
  }

  public override void Update(WarehouseItem entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<WarehouseItemDocument>(entity);
    var updateBuilder = Builders<WarehouseItemDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<WarehouseItemDocument>>();

    if (!string.IsNullOrWhiteSpace(document.WarehouseName))
      updateDefinitions.Add(updateBuilder.Set(d => d.WarehouseName, document.WarehouseName));

    if (!string.IsNullOrWhiteSpace(document.ProductName))
      updateDefinitions.Add(updateBuilder.Set(d => d.ProductName, document.ProductName));

    if (!string.IsNullOrWhiteSpace(document.Sku))
      updateDefinitions.Add(updateBuilder.Set(d => d.Sku, document.Sku));

    if (!string.IsNullOrWhiteSpace(document.VariantGroupId))
      updateDefinitions.Add(updateBuilder.Set(d => d.VariantGroupId, document.VariantGroupId));

    if (document.Quantity != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.Quantity, document.Quantity));

    if (document.WaitingForDelivery != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.WaitingForDelivery, document.WaitingForDelivery));

    updateDefinitions.Add(updateBuilder.Set(d => d.UpdatedAt, DateTime.UtcNow));

    if (updateDefinitions.Count == 0)
      return;

    var updateDefinition = updateBuilder.Combine(updateDefinitions);

    _context.AddCommand(session => _collection.UpdateOneAsync(
       session,
       d => d.Id == document.Id,
       updateDefinition,
       new UpdateOptions { IsUpsert = false }));
  }

  public override void Delete(WarehouseItem entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    _context.AddCommand(session => _collection.DeleteOneAsync(session, d => d.Id == entity.Id));
  }
}
