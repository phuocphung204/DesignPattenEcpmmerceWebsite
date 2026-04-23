using DesignPattern.Infrastructure.Mongo.Documents;
using DesignPattern.Infrastructure.Mongo.Abstractions;
using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.CollectionsConfiguration;

public class RatingCollectionConfiguration : IMongoCollectionConfiguration
{
  private readonly string _collectionName = "ratings";

  public async Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames)
  {
    if (!existingCollectionNames.Contains(_collectionName))
    {
      await database.CreateCollectionAsync(_collectionName);
    }

    var collection = database.GetCollection<RatingDocument>(_collectionName);
    await CreateIndexesAsync(collection);
  }

  private static async Task CreateIndexesAsync(IMongoCollection<RatingDocument> collection)
  {
    var UserAndProductIndexKeys = Builders<RatingDocument>.IndexKeys
      .Ascending(x => x.UserId)
      .Ascending(x => x.ProductId);
    var indexOptions = new CreateIndexOptions { Name = "idx_rating_user_id_product_id", Background = true };
    var indexModel = new CreateIndexModel<RatingDocument>(UserAndProductIndexKeys, indexOptions);
    await collection.Indexes.CreateOneAsync(indexModel);
  }
}