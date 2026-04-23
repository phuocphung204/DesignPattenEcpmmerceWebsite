using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class OrderCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "orders";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<OrderDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private static async Task CreateIndexesAsync(IMongoCollection<OrderDocument> collection)
  {
    var indexKeys = Builders<OrderDocument>.IndexKeys.Ascending(a => a.UserId);
    var indexOptions = new CreateIndexOptions { Name = "idx_user_id", Background = true };
    var indexModel = new CreateIndexModel<OrderDocument>(indexKeys, indexOptions);

    await collection.Indexes.CreateOneAsync(indexModel);
  }
}