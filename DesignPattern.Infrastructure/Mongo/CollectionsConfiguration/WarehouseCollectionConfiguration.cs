using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class WarehouseCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "warehouses";
  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<WarehouseDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private async Task CreateIndexesAsync(IMongoCollection<WarehouseDocument> collection)
  {
    var index = Builders<WarehouseDocument>.IndexKeys.Ascending(w => w.Name);
    var indexOptions = new CreateIndexOptions
    {
      Name = "idx_warehouse_name",
      Unique = true,
      Background = true
    };
    var indexModel = new CreateIndexModel<WarehouseDocument>(index, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}