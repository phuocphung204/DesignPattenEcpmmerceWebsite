using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo;

public class MongoDbInitializer
{
  private readonly IMongoDatabase _database;
  private readonly IEnumerable<IMongoCollectionConfiguration> _collectionConfigurations;
  public MongoDbInitializer(IMongoDatabase database, IEnumerable<IMongoCollectionConfiguration> collectionConfigurations)
  {
    _database = database;
    _collectionConfigurations = collectionConfigurations;
  }

  public async Task InitializeAsync()
  {
    var collectionNames = await _database.ListCollectionNames().ToListAsync();

    foreach (var configuration in _collectionConfigurations)
    {
      await configuration.ConfigureAsync(_database, collectionNames);
    }
  }
}