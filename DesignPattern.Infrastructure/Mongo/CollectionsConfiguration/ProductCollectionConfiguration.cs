using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class ProductCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "products";
  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<ProductDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }
  private async Task CreateIndexesAsync(IMongoCollection<ProductDocument> collection)
  {
    var indexKeys = Builders<ProductDocument>.IndexKeys.Ascending(a => a.Sku);
    var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_sku_unique", Background = true };
    var indexModel = new CreateIndexModel<ProductDocument>(indexKeys, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}