using MongoDB.Driver;
using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class DiscountCodeCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "discountCodes";
  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }
  }
  private async Task CreateIndexesAsync(IMongoCollection<DiscountCodeDocument> collection)
  {
    var indexKeys = Builders<DiscountCodeDocument>.IndexKeys.Ascending(a => a.Code);
    var indexOptions = new CreateIndexOptions { Unique = true, Name = "idx_code_unique", Background = true };
    var indexModel = new CreateIndexModel<DiscountCodeDocument>(indexKeys, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}