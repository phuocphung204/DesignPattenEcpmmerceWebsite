using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class UserCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "users";
  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<UserDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private async Task CreateIndexesAsync(IMongoCollection<UserDocument> collection)
  {
    var indexKeys = Builders<UserDocument>.IndexKeys.Ascending(a => a.Email);
    var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_email_unique", Background = true };
    var indexModel = new CreateIndexModel<UserDocument>(indexKeys, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}