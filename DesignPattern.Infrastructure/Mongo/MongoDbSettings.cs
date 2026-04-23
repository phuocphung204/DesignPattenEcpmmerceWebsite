namespace DesignPattern.Infrastructure.Mongo;

/// <summary>
/// Strongly-typed configuration object for MongoDB connection settings.
/// </summary>
public sealed class MongoDbSettings
{
  /// <summary>
  /// Connection string used by MongoClient.
  /// </summary>
  public string ConnectionString { get; init; } = string.Empty;

  /// <summary>
  /// Logical database name that repositories will use.
  /// </summary>
  public string DatabaseName { get; init; } = string.Empty;
}
