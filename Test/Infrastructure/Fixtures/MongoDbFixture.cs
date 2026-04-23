using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Test.Infrastructure.Fixtures;

// Kế thừa IAsyncLifetime của Xunit để Start/Stop container khi chạy test
public class MongoDbFixture : IAsyncLifetime
{
  private MongoDbContainer? _mongoContainer;

  public IMongoClient Client { get; private set; } = null!;
  public IMongoDatabase Database { get; private set; } = null!;

  public MongoDbFixture() { }

  public async Task InitializeAsync()
  {
    // Khởi tạo container dùng Image MongoDB mới nhất
    _mongoContainer = new MongoDbBuilder("mongo:6.0")
      // Nếu bạn dùng Transactions, cần set container chạy ở mode replica set
      .WithEnvironment("MONGO_REPLICA_SET_NAME", "rs0")
      .WithCommand("--replSet", "rs0", "--bind_ip_all", "--port", "27017")
      .Build();

    await _mongoContainer.StartAsync();


    // Đợi container và replica set khởi tạo xong
    await _mongoContainer.ExecAsync(new[] { "mongosh", "--eval", "rs.initiate();" });

    var connectionString = _mongoContainer.GetConnectionString();
    Client = new MongoClient(connectionString);

    // Tạo một database test
    Database = Client.GetDatabase("TestDb_DesignPattern");
  }

  public async Task DisposeAsync()
  {
    if (_mongoContainer is not null)
    {
      await _mongoContainer.DisposeAsync();
    }
  }
}

// Khai báo Collection để dùng chung cho tất cả các Test Repository
[CollectionDefinition("MongoDB Collection")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
  // Class rỗng dùng để Apply Fixture cho [Collection]
}
