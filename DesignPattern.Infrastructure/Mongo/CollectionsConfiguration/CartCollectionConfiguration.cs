using MongoDB.Driver;
using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class CartCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "carts";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<CartDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private async Task CreateIndexesAsync(IMongoCollection<CartDocument> collection)
  {
    var indexKeys = Builders<CartDocument>.IndexKeys.Ascending(a => a.UserId);
    var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_user_id_unique", Background = true };
    var indexModel = new CreateIndexModel<CartDocument>(indexKeys, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}
