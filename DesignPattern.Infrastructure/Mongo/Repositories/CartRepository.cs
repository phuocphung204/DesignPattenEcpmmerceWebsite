using AutoMapper;
using DesignPattern.Domain.Entities.Users;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

internal class CartRepository : MongoRepository<Cart, CartDocument>, ICartRepository
{
  public CartRepository(MongoContext context, IMapper mapper) : base(context, mapper, "carts") { }
  public async Task<Cart> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var filter = Builders<CartDocument>.Filter.Eq(c => c.UserId, userId);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Cart>(document);
  }

  public override async Task<Cart> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<CartDocument>.Filter.Eq(c => c.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<Cart>(document);
  }

  public override async Task<List<Cart>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var filter = Builders<CartDocument>.Filter.Empty;
    var documents = await _collection.Find(filter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<Cart>(doc)).ToList();
  }

  public override void Create(Cart entity)
  {
    var document = _mapper.Map<CartDocument>(entity);
    _context.AddCommand(session => _collection.InsertOneAsync(session, document));
  }

  public override void Update(Cart entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<CartDocument>(entity);
    var updateBuilder = Builders<CartDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<CartDocument>>();

    if (document.Items is { Count: > 0 })
      updateDefinitions.Add(updateBuilder.Set(x => x.Items, document.Items));

    if (updateDefinitions.Count == 0)
      return;

    var updateDefinition = updateBuilder.Combine(updateDefinitions);

    _context.AddCommand(session => _collection.UpdateOneAsync(
      session,
      x => x.Id == entity.Id,
      updateDefinition,
      new UpdateOptions { IsUpsert = false }));
  }

  public override void Delete(Cart entity)
  {
    // Ko xóa thể xóa cart của khách hàng, chỉ xóa item trong cart
    ArgumentNullException.ThrowIfNull(entity);
  }

}