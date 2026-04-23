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

internal class WarehouseRepository : MongoRepository<Warehouse, WarehouseDocument>, IWarehouseRepository
{
  private readonly IMongoCollection<WarehouseItemDocument> _warehouseItemsCollection;

  public WarehouseRepository(MongoContext context, IMapper mapper) : base(context, mapper, "warehouses")
  {
    _warehouseItemsCollection = context.GetCollection<WarehouseItemDocument>("warehouseItems");
  }

  public async Task<PagedResult<Warehouse>> GetPagedAsync(
    WarehouseCriteria criteria,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var safePageIndex = pageIndex < 1 ? 1 : pageIndex;
    var safePageSize = pageSize < 1 ? 10 : pageSize;

    var filterDefinitions = new List<FilterDefinition<WarehouseDocument>>();
    if (!string.IsNullOrWhiteSpace(criteria.Name))
    {
      var escapedName = Regex.Escape(criteria.Name.Trim());
      var namePattern = $".*{escapedName}.*";
      filterDefinitions.Add(Builders<WarehouseDocument>.Filter.Regex(
        d => d.Name,
        new BsonRegularExpression(namePattern, "i")));
    }
    if (!string.IsNullOrWhiteSpace(criteria.Address))
    {
      var escapedAddress = Regex.Escape(criteria.Address.Trim());
      var addressPattern = $".*{escapedAddress}.*";
      filterDefinitions.Add(Builders<WarehouseDocument>.Filter.Regex(
        d => d.Address,
        new BsonRegularExpression(addressPattern, "i")));
    }

    var filter = filterDefinitions.Any()
      ? Builders<WarehouseDocument>.Filter.And(filterDefinitions)
      : Builders<WarehouseDocument>.Filter.Empty;

    var totalCount = await _collection.CountDocumentsAsync(
      filter,
      cancellationToken: cancellationToken);

    var documents = await _collection
      .Find(filter)
      .Skip((safePageIndex - 1) * safePageSize)
      .Limit(safePageSize)
      .ToListAsync(cancellationToken);

    var items = documents
      .Select(doc => _mapper.Map<Warehouse>(doc))
      .ToList();

    return new PagedResult<Warehouse>
    {
      Items = items,
      TotalCount = (int)totalCount,
      PageIndex = safePageIndex,
      PageSize = safePageSize,
    };
  }

  public async Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseDocument>.Filter.Eq(c => c.Name, name);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Warehouse?>(document);
  }

  public override async Task<Warehouse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseDocument>.Filter.Eq(c => c.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Warehouse>(document);
  }

  public override async Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<WarehouseDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Warehouse>(doc)).ToList();
  }

  public override void Create(Warehouse entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<WarehouseDocument>(entity);
    _context.AddCommand(session => _collection.InsertOneAsync(session, document));
  }
  public override void Update(Warehouse entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<WarehouseDocument>(entity);
    var updateBuilder = Builders<WarehouseDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<WarehouseDocument>>();

    if (!string.IsNullOrWhiteSpace(document.Name))
      updateDefinitions.Add(updateBuilder.Set(d => d.Name, document.Name));

    if (!string.IsNullOrWhiteSpace(document.Address))
      updateDefinitions.Add(updateBuilder.Set(d => d.Address, document.Address));

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
  public override void Delete(Warehouse entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    _context.AddCommand(async session =>
    {
      var filter = Builders<WarehouseItemDocument>.Filter.Eq(d => d.WarehouseId, entity.Id);
      var hasItems = await _warehouseItemsCollection.Find(session, filter).AnyAsync();
      if (hasItems)
      {
        throw new InvalidOperationException($"Cannot delete warehouse {entity.Name} because it has associated items.");
      }

      await _collection.DeleteOneAsync(session, d => d.Id == entity.Id);
    });
  }
}
