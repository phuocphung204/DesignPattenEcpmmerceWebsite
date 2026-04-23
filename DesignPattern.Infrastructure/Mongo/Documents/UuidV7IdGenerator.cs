using MongoDB.Bson.Serialization;

namespace DesignPattern.Infrastructure.Mongo.Documents;

public class UuidV7IdGenerator : IIdGenerator
{
  public UuidV7IdGenerator() { }

  public object GenerateId(object container, object document)
  {
    return Guid.CreateVersion7();
  }

  public bool IsEmpty(object id)
  {
    if (id == null) return true;

    if (id is Guid guid)
    {
      return guid == Guid.Empty;
    }

    return false;
  }
}