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

internal class SeoRepository : MongoRepository<Seo, SeoDocument>, ISeoRepository
{
  public SeoRepository(MongoContext context, IMapper mapper) : base(context, mapper, "seos") { }

  public override async Task<Seo> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<SeoDocument>.Filter.Eq(s => s.Id, new ObjectId(id.ToString()));
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Seo>(document);
  }

  public override async Task<List<Seo>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<SeoDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Seo>(doc)).ToList();
  }

  public async Task<Seo> GetByReferenceIdAsync(Guid productId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<SeoDocument>.Filter.Eq(s => s.RefEntityId, productId);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Seo>(document);
  }
  public override void Create(Seo entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

  }
  public override void Update(Seo entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

  }
  public override void Delete(Seo entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

  }
}
