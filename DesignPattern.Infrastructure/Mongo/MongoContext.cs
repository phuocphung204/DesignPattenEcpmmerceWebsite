using MongoDB.Driver;

namespace DesignPattern.Infrastructure.Mongo;

public class MongoContext : IDisposable
{
  private readonly IMongoDatabase _database;
  private readonly IMongoClient _client;
  private IClientSessionHandle? _session { get; set; }
  private readonly List<Func<IClientSessionHandle, Task>> _commands = [];
  public MongoContext(IMongoClient client, IMongoDatabase database)
  {
    ArgumentNullException.ThrowIfNull(client);
    ArgumentNullException.ThrowIfNull(database);
    _client = client;
    _database = database;
  }

  public void AddCommand(Func<IClientSessionHandle, Task> command)
  {
    _commands.Add(command);
  }

  public void Dispose()
  {
    // Không cần dispose _client và _database vì chúng được quản lý bởi DI container và có thể được chia sẻ.
    // Chỉ cần xóa các lệnh đã lưu trữ để giải phóng tài nguyên nếu cần.
    _session?.Dispose();
    _commands.Clear();
    GC.SuppressFinalize(this);
  }

  public IMongoCollection<T> GetCollection<T>(string collectionName)
  {
    return _database.GetCollection<T>(collectionName);
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    using (_session = await _client.StartSessionAsync(cancellationToken: cancellationToken))
    {
      _session.StartTransaction();
      try
      {
        foreach (var command in _commands)
          await command(_session);
        await _session.CommitTransactionAsync(cancellationToken);
      }
      catch
      {
        await _session.AbortTransactionAsync(cancellationToken);
        throw;
      }
    }
    int affectedCommand = _commands.Count;
    _commands.Clear();
    return affectedCommand;
  }
}