using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class WarehouseItemCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "warehouseItems";
  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<WarehouseItemDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private async Task CreateIndexesAsync(IMongoCollection<WarehouseItemDocument> collection)
  {
    var nameIndexKeys = Builders<WarehouseItemDocument>.IndexKeys.Ascending(x => x.ProductName);
    var nameIndexOptions = new CreateIndexOptions
    {
      Name = "idx_warehouse_item_product_name",
      Background = true,
      Unique = false
    };
    var skuIndexKeys = Builders<WarehouseItemDocument>.IndexKeys.Ascending(x => x.Sku);
    var skuIndexOptions = new CreateIndexOptions
    {
      Name = "idx_warehouse_item_sku",
      Background = true,
      Unique = false
    };
    var warehouseProductUniqueIndexKeys = Builders<WarehouseItemDocument>.IndexKeys
      .Ascending(x => x.WarehouseId)
      .Ascending(x => x.ProductId);
    var warehouseProductUniqueIndexOptions = new CreateIndexOptions
    {
      Name = "uq_warehouse_item_warehouse_id_product_id",
      Background = true,
      Unique = true
    };

    await collection.Indexes.CreateManyAsync(
    [
      new CreateIndexModel<WarehouseItemDocument>(nameIndexKeys, nameIndexOptions),
      new CreateIndexModel<WarehouseItemDocument>(skuIndexKeys, skuIndexOptions),
      new CreateIndexModel<WarehouseItemDocument>(warehouseProductUniqueIndexKeys, warehouseProductUniqueIndexOptions)
    ]);

  }
}
