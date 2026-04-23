using System.Text.RegularExpressions;
using AutoMapper;
using DesignPattern.Domain.Common;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Enums;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

internal class ProductRepository : MongoRepository<Product, ProductDocument>, IProductRepository
{
  private readonly IMongoCollection<SeoDocument> _seoCollection;
  public ProductRepository(MongoContext context, IMapper mapper) : base(context, mapper, "products")
  {
    _seoCollection = context.GetCollection<SeoDocument>("seos");
  }

  public override async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var query = _collection.Aggregate()
      .Match(x => x.Id == id && x.Status == ProductStatus.Active)
      .Lookup<SeoDocument, ProductWithSeoRow>(
        foreignCollectionName: "seos",
        localField: "_id",
        foreignField: "refEntityId",
        @as: nameof(ProductWithSeoRow.Seos));

    var item = await query.FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Product>(item);
  }
  // search nhiều tiêu chí chuẩn string cho name nhưng trả về List<Product> đầy đủ thông tin product
  public async Task<List<Product>> GetProductsByVariantGroupIdAsync(string VariantGroupId, CancellationToken cancellationToken = default)
  {
    var filterBuilder = Builders<ProductDocument>.Filter;
    var filterDefinitions = new List<FilterDefinition<ProductDocument>>();

    filterDefinitions.Add(filterBuilder.Eq(x => x.Status, ProductStatus.Active));

    if (!string.IsNullOrWhiteSpace(VariantGroupId))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.VariantGroupId, VariantGroupId));
    }

    var filter = filterBuilder.And(filterDefinitions);

    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Product>(doc)).ToList();
  }
  // search nhiều tiêu chí gần đúng cho name nhưng trả về List<ProductReadModel> chỉ có một số thông tin cần thiết để hiển thị danh sách sản phẩm
  public async Task<PagedResult<ProductReadModel>> SearchProductsPagedAsync(
    ProductSearchCriteria criteria,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
  {
    var criteriaPageIndex = criteria.PageIndex > 0 ? criteria.PageIndex : pageIndex;
    var criteriaPageSize = criteria.PageSize > 0 ? criteria.PageSize : pageSize;
    var safePageIndex = criteriaPageIndex < 1 ? 1 : criteriaPageIndex;
    var safePageSize = criteriaPageSize < 1 ? 10 : criteriaPageSize;

    var filterBuilder = Builders<ProductDocument>.Filter;
    var filterDefinitions = new List<FilterDefinition<ProductDocument>>();

    filterDefinitions.Add(filterBuilder.Eq(x => x.Status, ProductStatus.Active));

    if (!string.IsNullOrWhiteSpace(criteria.Name))
    {
      var escapedName = Regex.Escape(criteria.Name.Trim());
      var namePattern = $".*{escapedName}.*";
      filterDefinitions.Add(filterBuilder.Regex(
        x => x.Name,
        new BsonRegularExpression(namePattern, "i")));
    }

    if (criteria.CategoryId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.CategoryId, criteria.CategoryId.Value));
    }

    if (criteria.BrandId.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.BrandId, criteria.BrandId.Value));
    }

    if (!string.IsNullOrWhiteSpace(criteria.VariantGroupId))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.VariantGroupId, criteria.VariantGroupId));
    }

    if (!string.IsNullOrWhiteSpace(criteria.Sku))
    {
      filterDefinitions.Add(filterBuilder.Eq(x => x.Sku, criteria.Sku));
    }

    if (criteria.MinRating.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Gte(x => x.Rating, criteria.MinRating.Value));
    }

    if (criteria.MaxRating.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Lte(x => x.Rating, criteria.MaxRating.Value));
    }

    if (criteria.MinPrice.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Gte(x => x.SellingPrice, criteria.MinPrice.Value));
    }

    if (criteria.MaxPrice.HasValue)
    {
      filterDefinitions.Add(filterBuilder.Lte(x => x.SellingPrice, criteria.MaxPrice.Value));
    }

    var filter = filterBuilder.And(filterDefinitions);

    var sortBuilder = Builders<ProductDocument>.Sort;
    var sortField = criteria.SortOption?.sortBy ?? Field.CreatedAt;
    var sortDirection = criteria.SortOption?.sortDirection ?? Direction.Desc;

    var sortDefinition = (sortField, sortDirection) switch
    {
      (Field.Price, Direction.Asc) => sortBuilder.Ascending(x => x.SellingPrice),
      (Field.Price, Direction.Desc) => sortBuilder.Descending(x => x.SellingPrice),
      (Field.Rating, Direction.Asc) => sortBuilder.Ascending(x => x.Rating),
      (Field.Rating, Direction.Desc) => sortBuilder.Descending(x => x.Rating),
      (Field.SoldQuantity, Direction.Asc) => sortBuilder.Ascending(x => x.SoldQuantity),
      (Field.SoldQuantity, Direction.Desc) => sortBuilder.Descending(x => x.SoldQuantity),
      (Field.CreatedAt, Direction.Asc) => sortBuilder.Ascending(x => x.CreatedAt),
      _ => sortBuilder.Descending(x => x.CreatedAt)
    };

    var totalCount = await _collection.CountDocumentsAsync(
      filter,
      cancellationToken: cancellationToken);

    var documents = await _collection
      .Find(filter)
      .Sort(sortDefinition)
      .Skip((safePageIndex - 1) * safePageSize)
      .Limit(safePageSize)
      .ToListAsync(cancellationToken);


    var items = documents
      .Select(doc => new ProductReadModel
      {
        Id = doc.Id,
        Name = doc.Name,
        Image = doc.Images != null && doc.Images.Count > 0 ? doc.Images[0] : string.Empty,
        SellingPrice = doc.SellingPrice,
        SoldQuantity = doc.SoldQuantity,
        Rating = doc.Rating,
        ShortDescription = doc.ShortDescription
      })
      .ToList();

    return new PagedResult<ProductReadModel>
    {
      Items = items,
      TotalCount = (int)totalCount,
      PageIndex = safePageIndex,
      PageSize = safePageSize
    };
  }
  public override async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var documents = await _collection.Aggregate()
      .Match(x => x.Status == ProductStatus.Active)
      .Lookup<SeoDocument, ProductWithSeoRow>(
        foreignCollectionName: "seos",
        localField: "_id",
        foreignField: "refEntityId",
        @as: nameof(ProductWithSeoRow.Seos))
      .SortByDescending(d => d.CreatedAt)
      .ToListAsync(cancellationToken);

    return documents.Select(doc => _mapper.Map<Product>(doc)).ToList();
  }

  public async Task<List<Product>> GetTopSellingProductsAsync(int topN, CancellationToken cancellationToken = default)
  {
    var filter = Builders<ProductDocument>.Filter.Eq(d => d.Status, ProductStatus.Active);
    var documents = await _collection.Find(filter)
      .SortByDescending(d => d.SoldQuantity)
      .Limit(topN)
      .ToListAsync(cancellationToken);

    return documents.Select(doc => _mapper.Map<Product>(doc)).ToList();
  }
  public override void Create(Product entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<ProductDocument>(entity);
    var seoDocument = _mapper.Map<SeoDocument>(entity.Seo);
    seoDocument.RefEntityId = document.Id;

    _context.AddCommand(async session =>
    {
      await _collection.InsertOneAsync(session, document);
      await _seoCollection.InsertOneAsync(session, seoDocument);
    });
  }
  public override void Update(Product entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var document = _mapper.Map<ProductDocument>(entity);
    var updateBuilder = Builders<ProductDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<ProductDocument>>();

    if (!string.IsNullOrWhiteSpace(document.Name))
      updateDefinitions.Add(updateBuilder.Set(d => d.Name, document.Name));

    if (document.BrandId != Guid.Empty)
      updateDefinitions.Add(updateBuilder.Set(d => d.BrandId, document.BrandId));

    if (!string.IsNullOrWhiteSpace(document.BrandName))
      updateDefinitions.Add(updateBuilder.Set(d => d.BrandName, document.BrandName));

    if (document.CategoryId != Guid.Empty)
      updateDefinitions.Add(updateBuilder.Set(d => d.CategoryId, document.CategoryId));

    if (!string.IsNullOrWhiteSpace(document.CategoryName))
      updateDefinitions.Add(updateBuilder.Set(d => d.CategoryName, document.CategoryName));

    if (!string.IsNullOrWhiteSpace(document.VariantGroupId))
      updateDefinitions.Add(updateBuilder.Set(d => d.VariantGroupId, document.VariantGroupId));

    if (!string.IsNullOrWhiteSpace(document.Sku))
      updateDefinitions.Add(updateBuilder.Set(d => d.Sku, document.Sku));

    if (document.Images is { Count: > 0 })
      updateDefinitions.Add(updateBuilder.Set(d => d.Images, document.Images));

    if (document.Attributes is { Count: > 0 })
      updateDefinitions.Add(updateBuilder.Set(d => d.Attributes, document.Attributes));

    if (document.SellingPrice != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.SellingPrice, document.SellingPrice));

    if (document.PurchasePrice != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.PurchasePrice, document.PurchasePrice));

    if (document.SoldQuantity != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.SoldQuantity, document.SoldQuantity));

    if (document.StockQuantity != 0)
      updateDefinitions.Add(updateBuilder.Set(d => d.StockQuantity, document.StockQuantity));

    if (document.Rating != 0.0m)
      updateDefinitions.Add(updateBuilder.Set(d => d.Rating, document.Rating));

    if (!string.IsNullOrWhiteSpace(document.ShortDescription))
      updateDefinitions.Add(updateBuilder.Set(d => d.ShortDescription, document.ShortDescription));

    if (!string.IsNullOrWhiteSpace(document.DetailDescription))
      updateDefinitions.Add(updateBuilder.Set(d => d.DetailDescription, document.DetailDescription));

    if (document.Status != default)
      updateDefinitions.Add(updateBuilder.Set(d => d.Status, document.Status));

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
  public override void Delete(Product entity)
  {
    // Xóa khỏi Product
    ArgumentNullException.ThrowIfNull(entity);
    _context.AddCommand(session => _collection.DeleteOneAsync(session, d => d.Id == entity.Id));
  }

  internal sealed class ProductWithSeoRow : ProductDocument
  {
    public SeoDocument? Seo { get; set; }
    public List<SeoDocument> Seos { get; set; } = [];
  }
}