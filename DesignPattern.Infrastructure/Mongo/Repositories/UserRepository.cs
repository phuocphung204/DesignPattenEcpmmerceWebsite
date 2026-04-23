using AutoMapper;
using DesignPattern.Domain.Entities.Users;
using System.Linq.Expressions;
using DesignPattern.Domain.Repositories;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;
using AutoMapper.Extensions.ExpressionMapping;
using MongoDB.Driver.Linq;

namespace DesignPattern.Infrastructure.Mongo.Repositories;

public sealed class UserRepository : MongoRepository<User, UserDocument>, IUserRepository
{
  private static readonly FilterDefinition<UserDocument> EmptyFilter = Builders<UserDocument>.Filter.Empty;

  public UserRepository(MongoContext context, IMapper mapper)
  : base(context, mapper, "users")
  { }

  public override async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var documents = await _collection.Find(EmptyFilter).SortByDescending(d => d.CreatedAt).ToListAsync(cancellationToken);
    return documents.Select(doc => _mapper.Map<User>(doc)).ToList();
  }

  public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<User>(document);
  }

  ///
  public async Task<User> GetUserWithSelectorAsync<TSelector>(
    Guid? userId,
    string? email,
    Expression<Func<User, TSelector>>? selector,
    CancellationToken cancellationToken = default)
  {
    if (selector is null)
    {
      if (userId is not null)
        return await GetByIdAsync(userId.Value, cancellationToken);

      if (!string.IsNullOrEmpty(email))
        return await GetByEmailAsync(email, cancellationToken);
      //TODO: chuyển throw vào Domain/Extension 
      throw new ArgumentException("At least one identifier (userId or email) must be provided.");
    }

    if (userId is null && string.IsNullOrEmpty(email))
      throw new ArgumentException("At least one identifier (userId or email) must be provided.");

    var docSelector = _mapper.MapExpression<Expression<Func<UserDocument, TSelector>>>(selector);
    Expression<Func<UserDocument, bool>> predicate = userId is not null
      ? doc => doc.Id == userId
      : doc => doc.Email == email;

    var query = _collection.AsQueryable()
      .Where(predicate)
      .Select(docSelector);

    Console.WriteLine($">>> Generated MongoDB query: {query}");
    var item = await query.FirstOrDefaultAsync(cancellationToken);
    Console.WriteLine($">>> Item is null: {item is null}");
    var itemCasted = item as UserDocument;
    Console.WriteLine($">>> Item casted to UserDocument is null: {itemCasted is null}");

    var user = _mapper.Map<User>(item);
    return user;
  }

  public async Task<User> GetAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, userId);

    var projection = Builders<UserDocument>.Projection
      .Include(x => x.Addresses)
      .Include(x => x.DefaultAddressId);

    var document = await _collection.Find(filter)
      .Project<UserDocument>(projection)
      .FirstOrDefaultAsync(cancellationToken);

    var user = _mapper.Map<User>(document);
    return user;
  }

  public override void Create(User entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<UserDocument>(entity);
    _context.AddCommand(session => _collection.InsertOneAsync(session, document));
  }

  public override void Update(User entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    var document = _mapper.Map<UserDocument>(entity);
    var updateBuilder = Builders<UserDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<UserDocument>>();

    if (!string.IsNullOrWhiteSpace(document.FullName))
      updateDefinitions.Add(updateBuilder.Set(x => x.FullName, document.FullName));

    if (!string.IsNullOrWhiteSpace(document.Email))
      updateDefinitions.Add(updateBuilder.Set(x => x.Email, document.Email));

    if (document.AvatarLink is not null)
      updateDefinitions.Add(updateBuilder.Set(x => x.AvatarLink, document.AvatarLink));

    if (document.DefaultAddressId != Guid.Empty)
      updateDefinitions.Add(updateBuilder.Set(x => x.DefaultAddressId, document.DefaultAddressId));

    if (document.Status != default)
      updateDefinitions.Add(updateBuilder.Set(x => x.Status, document.Status));

    if (document.Addresses is { Count: > 0 })
    {
      var docAddresses = _mapper.Map<List<VietNamAddress>>(document.Addresses);
      updateDefinitions.Add(updateBuilder.Set(x => x.Addresses, docAddresses));
    }

    if (document.LoyaltyPoints != default)
      updateDefinitions.Add(updateBuilder.Set(x => x.LoyaltyPoints, document.LoyaltyPoints));

    if (document.PasswordHash != default)
      updateDefinitions.Add(updateBuilder.Set(x => x.PasswordHash, document.PasswordHash));

    if (document.LinkedAccounts is { Count: > 0 })
    {
      var docLinkedAccounts = _mapper.Map<List<LinkedAccountDocument>>(document.LinkedAccounts);
      updateDefinitions.Add(updateBuilder.Set(x => x.LinkedAccounts, docLinkedAccounts));
    }

    if (updateDefinitions.Count == 0)
      return;

    var updateDefinition = updateBuilder.Combine(updateDefinitions);

    _context.AddCommand(session => _collection.UpdateOneAsync(
      session,
      x => x.Id == entity.Id,
      updateDefinition,
      new UpdateOptions { IsUpsert = false }));
    // IsUpsert = false có nghĩa là nếu không tìm thấy document nào có Id trùng với document.Id,
    // thì sẽ không thực hiện chèn mới (insert) mà sẽ trả về kết quả thất bại.
    // Nếu IsUpsert = true, thì nếu không tìm thấy document nào có Id trùng với document.Id,
    // MongoDB sẽ tự động chèn một document mới với dữ liệu từ document.
  }

  public void UpdateAddresses(User entity)
  {
    ArgumentNullException.ThrowIfNull(entity);

    var updateBuilder = Builders<UserDocument>.Update;
    var updateDefinitions = new List<UpdateDefinition<UserDocument>>
    {
      updateBuilder.Set(x => x.Addresses, _mapper.Map<List<VietNamAddress>>(entity.Addresses)),
      updateBuilder.Set(x => x.DefaultAddressId, entity.DefaultAddressId ?? Guid.Empty)
    };

    var updateDefinition = updateBuilder.Combine(updateDefinitions);

    _context.AddCommand(session => _collection.UpdateOneAsync(
      session,
      x => x.Id == entity.Id,
      updateDefinition,
      new UpdateOptions { IsUpsert = false }));
  }

  public override void Delete(User entity)
  {
    ArgumentNullException.ThrowIfNull(entity);
    _context.AddCommand(session => _collection.DeleteOneAsync(session, x => x.Id == entity.Id));
  }

  public override async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, id);
    var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    return _mapper.Map<User>(document);
  }

  public async Task<bool> IsEmailExistAsync(string email, CancellationToken cancellationToken)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);
    var exists = await _collection.Find(filter).AnyAsync(cancellationToken);
    return exists;
  }

  public async Task<bool> IsPhoneExistAsync(string phoneNumber, CancellationToken cancellationToken)
  {
    // var filter = Builders<UserDocument>.Filter.Eq(x => x.PhoneNumber, phoneNumber);
    // var exists = await _collection.Find(filter).AnyAsync(cancellationToken);
    // return exists;
    return false;
  }

  public async Task<bool> IsPhoneExistExcludingUserAsync(string phoneNumber, Guid excludeUserId, CancellationToken cancellationToken)
  {
    // var filter = Builders<UserDocument>.Filter.And(
    //   Builders<UserDocument>.Filter.Eq(x => x.PhoneNumber, phoneNumber),
    //   Builders<UserDocument>.Filter.Ne(x => x.Id, excludeUserId));
    // var exists = await _collection.Find(filter).AnyAsync(cancellationToken);
    // return exists;
    return false;
  }

  public async Task<User> GetAuthInfoByUserIdAsync(Guid userId, CancellationToken cancellationToken)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, userId);

    var projection = Builders<UserDocument>.Projection
      .Include(x => x.Id)
      .Include(x => x.Email)
      .Include(x => x.PasswordHash)
      .Include(x => x.Role)
      .Include(x => x.Status)
      .Include(x => x.FullName)
      .Include(x => x.AvatarLink)
      .Include(x => x.DefaultAddressId)
      .Include(x => x.Addresses);

    var document = await _collection.Find(filter)
      .Project<UserDocument>(projection)
      .FirstOrDefaultAsync(cancellationToken);

    var user = _mapper.Map<User>(document);
    return user;
  }

  public async Task<User> GetAuthInfoByEmailAsync(string email, CancellationToken cancellationToken)
  {
    var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);

    var projection = Builders<UserDocument>.Projection
      .Include(x => x.Id)
      .Include(x => x.Email)
      .Include(x => x.PasswordHash)
      .Include(x => x.Role)
      .Include(x => x.Status)
      .Include(x => x.FullName)
      .Include(x => x.AvatarLink);

    var document = await _collection.Find(filter)
      .Project<UserDocument>(projection)
      .FirstOrDefaultAsync(cancellationToken);

    var user = _mapper.Map<User>(document);
    return user;
  }

}
