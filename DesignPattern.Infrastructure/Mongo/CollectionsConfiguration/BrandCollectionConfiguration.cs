using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class BrandCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "brands";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<BrandDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private static async Task CreateIndexesAsync(IMongoCollection<BrandDocument> collection)
  {
    var indexKeys = Builders<BrandDocument>.IndexKeys.Ascending(a => a.Name);
    var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_brand_name_unique", Background = true };
    var indexModel = new CreateIndexModel<BrandDocument>(indexKeys, indexOptions);

    await collection.Indexes.CreateOneAsync(indexModel);
  }
}