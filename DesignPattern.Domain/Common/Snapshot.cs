namespace DesignPattern.Domain.Common;

public class SnapshotBase
{
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

  // public string Version { get; init; } = Guid.NewGuid().ToString();

  public SnapshotBase() { }
}

public interface ISnapshotable
{
  SnapshotBase CreateSnapshot();
}

