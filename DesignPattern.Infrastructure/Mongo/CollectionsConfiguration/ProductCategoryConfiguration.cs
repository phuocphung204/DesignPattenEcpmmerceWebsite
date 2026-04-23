using DesignPattern.Infrastructure.Mongo.Abstractions;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class ProductCategoryConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "productCategories";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<ProductCategoryDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private async Task CreateIndexesAsync(IMongoCollection<ProductCategoryDocument> collection)
  {
    var nameIndexKeys = Builders<ProductCategoryDocument>.IndexKeys.Ascending(a => a.Name);
    var nameIndexOptions = new CreateIndexOptions { Name = "idx_name_unique", Background = true };
    var nameIndexModel = new CreateIndexModel<ProductCategoryDocument>(nameIndexKeys, nameIndexOptions);

    // var parentCategoryIndexKeys = Builders<ProductCategoryDocument>.IndexKeys.Ascending(a => a.ParentCategoryId);
    // var parentCategoryIndexOptions = new CreateIndexOptions { Name = "idx_parent_category_id", Background = true };
    // var parentCategoryIndexModel = new CreateIndexModel<ProductCategoryDocument>(parentCategoryIndexKeys, parentCategoryIndexOptions);

    // await collection.Indexes.CreateManyAsync([nameIndexModel, parentCategoryIndexModel]);
  }
}
