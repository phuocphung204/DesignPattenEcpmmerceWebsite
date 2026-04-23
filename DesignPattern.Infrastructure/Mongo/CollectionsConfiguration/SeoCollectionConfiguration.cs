using DesignPattern.Infrastructure.Mongo.Abstractions;
using DesignPattern.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class SeoCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "seos";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<SeoDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private static async Task CreateIndexesAsync(IMongoCollection<SeoDocument> collection)
  {
    var refEntityIdIndexKeys = Builders<SeoDocument>.IndexKeys.Ascending(a => a.RefEntityId);
    var refEntityIdIndexOptions = new CreateIndexOptions { Unique = true, Name = "idx_ref_entity_id_unique", Background = true };
    var refEntityIdIndexModel = new CreateIndexModel<SeoDocument>(refEntityIdIndexKeys, refEntityIdIndexOptions);

    var slugIndexKeys = Builders<SeoDocument>.IndexKeys.Ascending(a => a.Slug);
    // sparse: true có nghĩa là chỉ tạo index 
    // cho các document có trường Slug tồn tại và có giá trị khác null.
    var slugIndexOptions = new CreateIndexOptions { Unique = true, Name = "idx_slug_unique", Background = true, Sparse = true };
    var slugIndexModel = new CreateIndexModel<SeoDocument>(slugIndexKeys, slugIndexOptions);

    await collection.Indexes.CreateManyAsync([refEntityIdIndexModel, slugIndexModel]);
  }
}
