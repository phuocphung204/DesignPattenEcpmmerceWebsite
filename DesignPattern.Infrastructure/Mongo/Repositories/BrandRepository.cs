using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

public sealed class BrandRepository : MongoRepository<Brand, BrandDocument>, IBrandRepository
{
  private readonly IMongoCollection<SeoDocument> _seoCollection;

  public BrandRepository(MongoContext context, IMapper mapper)
      : base(context, mapper, "brands")
  {
    _seoCollection = context.GetCollection<SeoDocument>("seos");
  }

  public new async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
      Expression<Func<Brand, TResult>> selector,
      Expression<Func<Brand, bool>>? predicate = null,
      Expression<Func<IQueryable<Brand>, IOrderedQueryable<Brand>>>? orderBy = null,
      int pageIndex = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default,
      params string[] includeProperties)
  {
    ArgumentNullException.ThrowIfNull(selector);

    var isSeoSelected = selector.IsPropertySelected("Seo");
    var query = _collection.AsQueryable();

    if (isSeoSelected)
    {
      var joinedQuery = query
          .Join(
              inner: _seoCollection.AsQueryable(),
              outerKeySelector: brand => brand.Id,
              innerKeySelector: seo => seo.RefEntityId,
              resultSelector: (brand, seo) => new BrandWithSeoRow
              {
                Id = brand.Id,
                Name = brand.Name,
                Level = brand.Level,
                ParentId = brand.ParentId,
                Seo = seo,
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
              });

      var docSelector = _mapper.MapExpression<Expression<Func<BrandWithSeoRow, TResult>>>(selector);
      var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<BrandWithSeoRow, bool>>>(predicate) : null;
      var docOrderBy = orderBy != null ? _mapper.MapExpression<Expression<Func<IQueryable<BrandWithSeoRow>, IOrderedQueryable<BrandWithSeoRow>>>>(orderBy).Compile() : null;

      if (docPredicate != null) joinedQuery = joinedQuery.Where(docPredicate);
      int totalCount = await joinedQuery.CountAsync(cancellationToken);

      if (docOrderBy != null) joinedQuery = docOrderBy(joinedQuery);

      var finalQuery = joinedQuery
          .Select(docSelector)
          .Skip((pageIndex - 1) * pageSize)
          .Take(pageSize);

      var items = await finalQuery.ToListAsync(cancellationToken);

      return new PagedResult<TResult>
      {
        Items = items,
        TotalCount = totalCount,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
    }
    else
    {
      var docSelector = _mapper.MapExpression<Expression<Func<BrandDocument, TResult>>>(selector);
      var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<BrandDocument, bool>>>(predicate) : null;
      var docOrderBy = orderBy != null ? _mapper.MapExpression<Expression<Func<IQueryable<BrandDocument>, IOrderedQueryable<BrandDocument>>>>(orderBy).Compile() : null;

      if (docPredicate != null) query = query.Where(docPredicate);
      int totalCount = await query.CountAsync(cancellationToken);

      if (docOrderBy != null) query = docOrderBy(query);

      var finalQuery = query
          .Select(docSelector)
          .Skip((pageIndex - 1) * pageSize)
          .Take(pageSize);

      var items = await finalQuery.ToListAsync(cancellationToken);

      return new PagedResult<TResult>
      {
        Items = items,
        TotalCount = totalCount,
        PageIndex = pageIndex,
        PageSize = pageSize
      };
    }
  }

  public override async Task<List<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var documents = await _collection.Aggregate()
        .Lookup<SeoDocument, BrandWithSeoRow>(
            foreignCollectionName: "seos",
            localField: "_id",
            foreignField: "refEntityId",
            @as: nameof(BrandWithSeoRow.Seos))
        .SortByDescending(d => d.CreatedAt)
        .ToListAsync(cancellationToken);

    return documents
        .Select(doc => (Brand)_mapper.Map<Brand>(doc))
        .ToList();
  }

  public override async Task<Brand> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var query = _collection.Aggregate()
      .Match(x => x.Id == id)
      .Lookup<SeoDocument, BrandWithSeoRow>(
        foreignCollectionName: "seos",
        localField: "_id",
        foreignField: "refEntityId",
        @as: nameof(BrandWithSeoRow.Seos));

    var item = await query.FirstOrDefaultAsync(cancellationToken);

    var brand = _mapper.Map<Brand>(item);

    return brand;
  }

  public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    var document = await _collection.Find(x => x.Name == name).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Brand>(document);
  }

  public override void Create(Brand entity)
  {
    var document = _mapper.Map<BrandDocument>(entity);
    var seoDocument = _mapper.Map<SeoDocument>(entity.Seo);
    seoDocument.RefEntityId = document.Id;

    _context.AddCommand(async session =>
    {
      await _collection.InsertOneAsync(session, document);
      await _seoCollection.InsertOneAsync(session, seoDocument);
    });
  }

  public override void Update(Brand entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var document = _mapper.Map<BrandDocument>(entity);
    var updateBrandBuilder = Builders<BrandDocument>.Update;
    var updateBrandDefinitions = new List<UpdateDefinition<BrandDocument>>();

    if (!string.IsNullOrWhiteSpace(document.Name))
      updateBrandDefinitions.Add(updateBrandBuilder.Set(x => x.Name, document.Name));

    if (document.Level != 0)
      updateBrandDefinitions.Add(updateBrandBuilder.Set(x => x.Level, document.Level));

    if (document.ParentId.HasValue)
      updateBrandDefinitions.Add(updateBrandBuilder.Set(x => x.ParentId, document.ParentId));

    updateBrandDefinitions.Add(updateBrandBuilder.Set(x => x.UpdatedAt, DateTime.UtcNow));

    var updateSeoBuilder = Builders<SeoDocument>.Update;
    var updateSeoDefinitions = new List<UpdateDefinition<SeoDocument>>();

    if (!string.IsNullOrWhiteSpace(entity.Seo.Title?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.MetaTitle, entity.Seo.Title!.Value));

    if (!string.IsNullOrWhiteSpace(entity.Seo.Description?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.MetaDescription, entity.Seo.Description!.Value));

    if (!string.IsNullOrWhiteSpace(entity.Seo.Slug?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.Slug, entity.Seo.Slug!.Value));

    var updateBrandDefinition = updateBrandBuilder.Combine(updateBrandDefinitions);
    var updateSeoDefinition = updateSeoDefinitions.Count > 0
      ? updateSeoBuilder.Combine(updateSeoDefinitions)
      : null;

    _context.AddCommand(async session =>
    {
      await _collection.UpdateOneAsync(
        session,
        x => x.Id == document.Id,
        updateBrandDefinition,
        new UpdateOptions { IsUpsert = false });

      if (updateSeoDefinition is not null)
      {
        await _seoCollection.UpdateOneAsync(
          session,
          x => x.RefEntityId == document.Id,
          updateSeoDefinition,
          new UpdateOptions { IsUpsert = false });
      }
    });
  }

  public override void Delete(Brand entity)
  {
    var id = entity.Id;
    _context.AddCommand(async session =>
    {
      await _collection.DeleteOneAsync(session, x => x.Id == id);
      await _seoCollection.DeleteOneAsync(session, x => x.RefEntityId == id);
    });
  }

  public async Task<PagedResult<BrandReadModel>> SearchBrandsPagedAsync(
      string? nameSearchTerm = null,
      int pageIndex = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default)
  {
    var pipeline = _collection.Aggregate();

    var selected = pipeline.Project(doc => new BrandDocument
    {
      Id = doc.Id,
      Name = doc.Name,
      Level = doc.Level,
      ParentId = doc.ParentId
    });

    var withSeo = selected
        .Lookup<BrandDocument, SeoDocument, BrandWithJoinsRow>(
            foreignCollection: _seoCollection,
            localField: doc => doc.Id,
            foreignField: seo => seo.RefEntityId,
            @as: row => row.SeoData);

    var withParent = withSeo
        .Lookup<BrandWithJoinsRow, BrandDocument, BrandWithJoinsRow>(
            foreignCollection: _collection,
            localField: row => row.ParentId,
            foreignField: parent => parent.Id,
            @as: row => row.ParentData);

    IAggregateFluent<BrandWithJoinsRow> filtered = withParent;
    if (!string.IsNullOrWhiteSpace(nameSearchTerm))
    {
      var escapedTerm = Regex.Escape(nameSearchTerm);
      var regex = new BsonRegularExpression(escapedTerm, "i");
      var nameFilter = Builders<BrandWithJoinsRow>.Filter.Regex(x => x.Name, regex);
      filtered = filtered.Match(nameFilter);
    }

    var countFacet = AggregateFacet.Create("count",
        PipelineDefinition<BrandWithJoinsRow, AggregateCountResult>.Create(new[]
        {
          PipelineStageDefinitionBuilder.Count<BrandWithJoinsRow>()
        }));

    var dataFacet = AggregateFacet.Create("data",
        PipelineDefinition<BrandWithJoinsRow, BrandWithJoinsRow>.Create(new[]
        {
          PipelineStageDefinitionBuilder.Skip<BrandWithJoinsRow>((pageIndex - 1) * pageSize),
          PipelineStageDefinitionBuilder.Limit<BrandWithJoinsRow>(pageSize)
        }));

    var facetResult = await filtered
      .Facet(countFacet, dataFacet)
      .FirstOrDefaultAsync(cancellationToken);

    var totalCount = 0;
    var countOutput = facetResult?.Facets
      .FirstOrDefault(f => f.Name == "count")?
      .Output<AggregateCountResult>();
    if (countOutput != null && countOutput.Any())
    {
      totalCount = (int)countOutput.First().Count;
    }

    var rows = facetResult?.Facets
      .FirstOrDefault(f => f.Name == "data")?
      .Output<BrandWithJoinsRow>() ?? new List<BrandWithJoinsRow>();

    var items = rows.Select(row =>
    {
      var seo = row.SeoData?.FirstOrDefault();
      var parent = row.ParentData?.FirstOrDefault();

      return new BrandReadModel
      {
        Id = row.Id,
        Name = row.Name,
        Slug = seo?.Slug,
        Level = row.Level,
        ParentBrandId = parent != null ? parent.Id : null,
        ParentBrandName = parent?.Name
      };
    }).ToList();

    return new PagedResult<BrandReadModel>
    {
      Items = items,
      TotalCount = totalCount,
      PageIndex = pageIndex,
      PageSize = pageSize
    };
  }

  internal sealed class BrandWithSeoRow : BrandDocument
  {
    public SeoDocument? Seo { get; set; }
    public List<SeoDocument> Seos { get; set; } = [];
  }

  internal sealed class BrandWithJoinsRow : BrandDocument
  {
    public List<SeoDocument>? SeoData { get; set; }
    public List<BrandDocument>? ParentData { get; set; }
  }
}
