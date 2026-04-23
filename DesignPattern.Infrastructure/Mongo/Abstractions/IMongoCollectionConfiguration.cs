using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo.Abstractions;

public interface IMongoCollectionConfiguration
{
  Task ConfigureAsync(IMongoDatabase database, List<string> existingCollectionNames);
}