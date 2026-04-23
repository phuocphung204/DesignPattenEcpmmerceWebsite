using AutoMapper;
using DesignPattern.Domain.Entities;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

internal class RatingRepository : MongoRepository<Rating, RatingDocument>, IRatingRepository
{
  public RatingRepository(MongoContext context, IMapper mapper) : base(context, mapper, "ratings") { }

  public async Task<List<Rating>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<RatingDocument>.Filter.Eq(c => c.ProductId, productId);
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Rating>(doc)).ToList();
  }
  public override async Task<Rating> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<RatingDocument>.Filter.Eq(c => c.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Rating>(document);
  }
  public override async Task<List<Rating>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<RatingDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Rating>(doc)).ToList();
  }

  public override void Create(Rating entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<RatingDocument>(entity);
    _context.AddCommand(session => _collection.InsertOneAsync(session, document));
  }

  public override void Update(Rating entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var document = _mapper.Map<RatingDocument>(entity);
    var updateBuilder = Builders<RatingDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<RatingDocument>>();

    if (document.Value != 0)
      updateDefinitions.Add(updateBuilder.Set(x => x.Value, document.Value));

    if (!string.IsNullOrEmpty(document.Comment))
      updateDefinitions.Add(updateBuilder.Set(x => x.Comment, document.Comment));

    if (updateDefinitions.Count == 0)
      return;

    var updateDefinition = updateBuilder.Combine(updateDefinitions);

    _context.AddCommand(session => _collection.UpdateOneAsync(
        session,
        c => c.Id == entity.Id,
        updateDefinition,
        new UpdateOptions { IsUpsert = false }));
  }

  public override void Delete(Rating entity)
  {
    _context.AddCommand(session => _collection.DeleteOneAsync(session, c => c.Id == entity.Id));
  }
}