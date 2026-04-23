using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities.ProductCategory;
using DesignPattern.Domain.Extensions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace DesignPattern.Infrastructure.Mongo.Repositories;

public sealed class ProductCategoryRepository : MongoRepository<ProductCategory, ProductCategoryDocument>, IProductCategoryRepository
{
  private IMongoCollection<SeoDocument> _seoCollection { get; init; }

  public ProductCategoryRepository(MongoContext context, IMapper mapper)
    : base(context, mapper, "productCategories")
  {
    _seoCollection = context.GetCollection<SeoDocument>("seos");
  }

  public new async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
    Expression<Func<ProductCategory, TResult>> selector,
    Expression<Func<ProductCategory, bool>>? predicate = null,
    Expression<Func<IQueryable<ProductCategory>, IOrderedQueryable<ProductCategory>>>? orderBy = null,
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
      Console.WriteLine("Seo selected, joining...");
      var joinedQuery = query
        .Join(
          inner: _seoCollection.AsQueryable(),
          outerKeySelector: pro => pro.Id,
          innerKeySelector: seo => seo.RefEntityId,
          resultSelector: (pro, seo) => new ProductCategoryWithSeoRow
          {
            Id = pro.Id,
            Name = pro.Name,
            Level = pro.Level,
            ParentCategoryId = pro.ParentCategoryId,
            Seo = seo,
            CreatedAt = pro.CreatedAt,
            UpdatedAt = pro.UpdatedAt
          });

      var docSelector = _mapper.MapExpression<Expression<Func<ProductCategoryWithSeoRow, TResult>>>(selector);
      var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<ProductCategoryWithSeoRow, bool>>>(predicate) : null;
      var docOrderBy = orderBy != null ? _mapper.MapExpression<Expression<Func<IQueryable<ProductCategoryWithSeoRow>, IOrderedQueryable<ProductCategoryWithSeoRow>>>>(orderBy).Compile() : null;

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
      var docSelector = _mapper.MapExpression<Expression<Func<ProductCategoryDocument, TResult>>>(selector);
      var docPredicate = predicate != null ? _mapper.MapExpression<Expression<Func<ProductCategoryDocument, bool>>>(predicate) : null;
      var docOrderBy = orderBy != null ? _mapper.MapExpression<Expression<Func<IQueryable<ProductCategoryDocument>, IOrderedQueryable<ProductCategoryDocument>>>>(orderBy).Compile() : null;

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

  public override async Task<List<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var documents = await _collection.Aggregate()
      .Lookup<SeoDocument, ProductCategoryWithSeoRow>(
        foreignCollectionName: "seos",
        localField: "_id",
        foreignField: "refEntityId",
        @as: nameof(ProductCategoryWithSeoRow.Seos))
      .SortByDescending(d => d.CreatedAt)
      .ToListAsync(cancellationToken);

    var result = documents
      .Select(doc => (ProductCategory)_mapper.Map<ProductCategory>(doc))
      .ToList();

    return result;
  }

  public override async Task<ProductCategory> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var query = _collection.Aggregate()
      .Match(x => x.Id == id)
      .Lookup<SeoDocument, ProductCategoryWithSeoRow>(
        foreignCollectionName: "seos",
        localField: "_id",
        foreignField: "refEntityId",
        @as: nameof(ProductCategoryWithSeoRow.Seos));

    var item = await query.FirstOrDefaultAsync(cancellationToken);

    // Console.WriteLine($"{item.Seos[0].MetaDescription}, {item.Seos[0].MetaTitle}, {item.Seos[0].Slug}");

    var productCategory = _mapper.Map<ProductCategory>(item);

    return productCategory;
  }

  public async Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
  {
    // TODO: GetByNameAsync
    throw new NotImplementedException();
  }

  public override void Create(ProductCategory entity)
  {
    var document = _mapper.Map<ProductCategoryDocument>(entity);
    var seoDocument = _mapper.Map<SeoDocument>(entity.Seo);
    seoDocument.RefEntityId = document.Id;

    _context.AddCommand(async session =>
    {
      await _collection.InsertOneAsync(session, document);
      await _seoCollection.InsertOneAsync(session, seoDocument);
    });
  }

  public override void Update(ProductCategory entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var document = _mapper.Map<ProductCategoryDocument>(entity);
    var updateCategoryBuilder = Builders<ProductCategoryDocument>.Update;
    var updateCategoryDefinitions = new List<UpdateDefinition<ProductCategoryDocument>>();

    if (!string.IsNullOrWhiteSpace(document.Name))
      updateCategoryDefinitions.Add(updateCategoryBuilder.Set(x => x.Name, document.Name));

    if (document.Level != 0)
      updateCategoryDefinitions.Add(updateCategoryBuilder.Set(x => x.Level, document.Level));

    if (document.ParentCategoryId.HasValue)
      updateCategoryDefinitions.Add(updateCategoryBuilder.Set(x => x.ParentCategoryId, document.ParentCategoryId));

    updateCategoryDefinitions.Add(updateCategoryBuilder.Set(x => x.UpdatedAt, DateTime.UtcNow));

    var updateSeoBuilder = Builders<SeoDocument>.Update;
    var updateSeoDefinitions = new List<UpdateDefinition<SeoDocument>>();

    if (!string.IsNullOrWhiteSpace(entity.Seo.Title?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.MetaTitle, entity.Seo.Title!.Value));

    if (!string.IsNullOrWhiteSpace(entity.Seo.Description?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.MetaDescription, entity.Seo.Description!.Value));

    if (!string.IsNullOrWhiteSpace(entity.Seo.Slug?.Value))
      updateSeoDefinitions.Add(updateSeoBuilder.Set(x => x.Slug, entity.Seo.Slug!.Value));

    var updateCategoryDefinition = updateCategoryBuilder.Combine(updateCategoryDefinitions);
    var updateSeoDefinition = updateSeoDefinitions.Count > 0
      ? updateSeoBuilder.Combine(updateSeoDefinitions)
      : null;

    _context.AddCommand(async session =>
    {
      await _collection.UpdateOneAsync(
        session,
        x => x.Id == document.Id,
        updateCategoryDefinition,
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

  public override void Delete(ProductCategory entity)
  {
    var _id = entity.Id;
    _context.AddCommand(async session =>
    {
      await _collection.DeleteOneAsync(session, x => x.Id == _id);
      await _seoCollection.DeleteOneAsync(session, x => x.RefEntityId == _id);
    });
  }

  public async Task<PagedResult<ProductCategoryReadModel>> SearchCategoriesPagedAsync(
    string? nameSearchTerm = null,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    // Bắt đầu aggregation pipeline
    var pipeline = _collection.Aggregate();

    // $lookup: Left outer join với collection "seos" để lấy Slug
    var selected = pipeline.Project(doc => new ProductCategoryDocument
    {
      Id = doc.Id,
      Name = doc.Name,
      Level = doc.Level,
      ParentCategoryId = doc.ParentCategoryId
    });
    var withSeo = selected
      .Lookup<ProductCategoryDocument, SeoDocument, CategoryWithJoinsRow>(
        foreignCollection: _seoCollection,
        localField: doc => doc.Id,
        foreignField: seo => seo.RefEntityId,
        @as: row => row.SeoData);

    // $lookup: Left outer join với collection "productCategories" để lấy thông tin danh mục cha
    var withParent = withSeo
      .Lookup<CategoryWithJoinsRow, ProductCategoryDocument, CategoryWithJoinsRow>(
        foreignCollection: _collection,
        localField: row => row.ParentCategoryId,
        foreignField: parent => parent.Id,
        @as: row => row.ParentData);

    // $match: Tìm kiếm theo tên (partial match, case-insensitive)
    IAggregateFluent<CategoryWithJoinsRow> filtered = withParent;
    if (!string.IsNullOrWhiteSpace(nameSearchTerm))
    {
      var escapedTerm = System.Text.RegularExpressions.Regex.Escape(nameSearchTerm);
      var regex = new MongoDB.Bson.BsonRegularExpression(escapedTerm, "i");
      var nameFilter = Builders<CategoryWithJoinsRow>.Filter.Regex(x => x.Name, regex);
      filtered = filtered.Match(nameFilter);
    }

    // Đếm tổng số bản ghi sau khi filter
    // Tạo 2 nhánh song song: 1 nhánh đếm, 1 nhánh lấy data phân trang
    var countFacet = AggregateFacet.Create("count",
      PipelineDefinition<CategoryWithJoinsRow, AggregateCountResult>.Create(new[]
      {
        PipelineStageDefinitionBuilder.Count<CategoryWithJoinsRow>()
      }));

    var dataFacet = AggregateFacet.Create("data",
      PipelineDefinition<CategoryWithJoinsRow, CategoryWithJoinsRow>.Create(new[]
      {
        PipelineStageDefinitionBuilder.Skip<CategoryWithJoinsRow>((pageIndex - 1) * pageSize),
        PipelineStageDefinitionBuilder.Limit<CategoryWithJoinsRow>(pageSize)
      }));

    var facetResult = await filtered
      .Facet(countFacet, dataFacet)
      .FirstOrDefaultAsync(cancellationToken);

    // Lấy totalCount từ facet "count"
    var totalCount = 0;
    var countOutput = facetResult?.Facets
      .FirstOrDefault(f => f.Name == "count")?
      .Output<AggregateCountResult>();
    if (countOutput != null && countOutput.Any())
    {
      totalCount = (int)countOutput.First().Count;
    }

    // Lấy danh sách items từ facet "data"
    var rows = facetResult?.Facets
      .FirstOrDefault(f => f.Name == "data")?
      .Output<CategoryWithJoinsRow>() ?? new List<CategoryWithJoinsRow>();

    // Map từ aggregate row sang read model
    var items = rows.Select(row =>
    {
      var seo = row.SeoData?.FirstOrDefault();
      var parent = row.ParentData?.FirstOrDefault();

      return new ProductCategoryReadModel
      {
        Id = row.Id,
        Name = row.Name,
        Slug = seo?.Slug,
        Level = row.Level,
        ParentCategoryId = parent != null ? parent.Id : null,
        ParentCategoryName = parent?.Name
      };
    }).ToList();

    return new PagedResult<ProductCategoryReadModel>
    {
      Items = items,
      TotalCount = totalCount,
      PageIndex = pageIndex,
      PageSize = pageSize
    };
  }



  internal sealed class ProductCategoryWithSeoRow : ProductCategoryDocument
  {
    public SeoDocument? Seo { get; set; }
    public List<SeoDocument> Seos { get; set; } = [];
  }

  /// <summary>
  /// Row trung gian chứa kết quả sau khi $lookup cả Seo và Parent Category.
  /// </summary>
  internal sealed class CategoryWithJoinsRow : ProductCategoryDocument
  {
    public List<SeoDocument>? SeoData { get; set; }
    public List<ProductCategoryDocument>? ParentData { get; set; }
  }

}

