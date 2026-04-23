using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.DiscountCodes;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

public sealed class DiscountCodeRepository : MongoRepository<DiscountCode, DiscountCodeDocument>, IDiscountCodeRepository
{
  public DiscountCodeRepository(MongoContext context, IMapper mapper)
      : base(context, mapper, "discountCodes")
  { }

  public override async Task<List<DiscountCode>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<DiscountCodeDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<DiscountCode>(doc)).ToList();
  }

  public override async Task<DiscountCode> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var document = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    return _mapper.Map<DiscountCode>(document);
  }

  public async Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
  {
    var document = await _collection.Find(x => x.Code == code).FirstOrDefaultAsync(cancellationToken);
    if (document is null) return null;

    return _mapper.Map<DiscountCode>(document);
  }

  public override void Create(DiscountCode entity) // khi dùng: truyền trực tiếp vào subclass
  {
    var document = _mapper.Map<DiscountCodeDocument>(entity);
    _context.AddCommand(async session =>
    {
      await _collection.InsertOneAsync(session, document);
    });
  }

  public override void Update(DiscountCode entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var document = _mapper.Map<DiscountCodeDocument>(entity);
    var updateBuilder = Builders<DiscountCodeDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<DiscountCodeDocument>>();

    if (!string.IsNullOrWhiteSpace(document.Code))
      updateDefinitions.Add(updateBuilder.Set(d => d.Code, document.Code));

    if (document.ExpirationDate != default)
      updateDefinitions.Add(updateBuilder.Set(d => d.ExpirationDate, document.ExpirationDate));

    if (document.UsageLimit != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.UsageLimit, document.UsageLimit));

    if (document.UsedCount != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.UsedCount, document.UsedCount));

    if (document.MinimumOrderAmount != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.MinimumOrderAmount, document.MinimumOrderAmount));

    if (document.Type != default)
      updateDefinitions.Add(updateBuilder.Set(d => d.Type, document.Type));

    if (document.Amount.HasValue)
      updateDefinitions.Add(updateBuilder.Set(d => d.Amount, document.Amount));

    if (document.Percent.HasValue)
      updateDefinitions.Add(updateBuilder.Set(d => d.Percent, document.Percent));

    if (document.MaximumDiscountAmount.HasValue)
      updateDefinitions.Add(updateBuilder.Set(d => d.MaximumDiscountAmount, document.MaximumDiscountAmount));

    updateDefinitions.Add(updateBuilder.Set(d => d.IsActive, document.IsActive));
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

  public override void Delete(DiscountCode entity)
  {
    var id = entity.Id;
    _context.AddCommand(async session =>
    {
      await _collection.DeleteOneAsync(session, x => x.Id == id);
    });
  }

  public async Task<PagedResult<DiscountCodeReadModel>> SearchDiscountCodesPagedAsync(
      string? searchTerm = null,
      bool? isActive = null,
      DateTime? startDate = null,
      DateTime? endDate = null,
      int pageIndex = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default)
  {
    var builder = Builders<DiscountCodeDocument>.Filter;
    var filter = builder.Empty;

    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
      var escapedTerm = System.Text.RegularExpressions.Regex.Escape(searchTerm);
      var regex = new BsonRegularExpression(escapedTerm, "i");
      filter &= builder.Regex(x => x.Code, regex);
    }

    if (isActive.HasValue)
    {
      filter &= builder.Eq(x => x.IsActive, isActive.Value);
    }

    if (startDate.HasValue)
    {
      filter &= builder.Gte(x => x.CreatedAt, startDate.Value);
    }

    if (endDate.HasValue)
    {
      filter &= builder.Lte(x => x.CreatedAt, endDate.Value);
    }

    var countFacet = AggregateFacet.Create("count",
        PipelineDefinition<DiscountCodeDocument, AggregateCountResult>.Create(new[]
        {
            PipelineStageDefinitionBuilder.Count<DiscountCodeDocument>()
        }));

    var dataFacet = AggregateFacet.Create("data",
        PipelineDefinition<DiscountCodeDocument, DiscountCodeDocument>.Create(new[]
        {
            PipelineStageDefinitionBuilder.Sort(Builders<DiscountCodeDocument>.Sort.Descending(x => x.CreatedAt)),
            PipelineStageDefinitionBuilder.Skip<DiscountCodeDocument>((pageIndex - 1) * pageSize),
            PipelineStageDefinitionBuilder.Limit<DiscountCodeDocument>(pageSize)
        }));

    var aggregation = await _collection.Aggregate()
        .Match(filter)
        .Facet(countFacet, dataFacet)
        .FirstOrDefaultAsync(cancellationToken);

    var totalCount = 0;
    var countOutput = aggregation?.Facets
        .FirstOrDefault(f => f.Name == "count")?
        .Output<AggregateCountResult>();

    if (countOutput != null && countOutput.Any())
    {
      totalCount = (int)countOutput.First().Count;
    }

    var rows = aggregation?.Facets
        .FirstOrDefault(f => f.Name == "data")?
        .Output<DiscountCodeDocument>() ?? new List<DiscountCodeDocument>();

    var items = rows.Select(doc => new DiscountCodeReadModel
    {
      Id = doc.Id,
      Code = doc.Code,
      CreatedAt = doc.CreatedAt,
      ExpirationDate = doc.ExpirationDate,
      IsActive = doc.IsActive,
      UsageLimit = doc.UsageLimit,
      UsedCount = doc.UsedCount,
      MinimumOrderAmount = doc.MinimumOrderAmount,
      MaximumDiscountAmount = doc.MaximumDiscountAmount
    }).ToList();

    return new PagedResult<DiscountCodeReadModel>
    {
      Items = items,
      TotalCount = totalCount,
      PageIndex = pageIndex,
      PageSize = pageSize
    };
  }
}
