using Xunit;

namespace Test.Infrastructure.Fixtures;

public sealed class DockerFactAttribute : FactAttribute
{
  public DockerFactAttribute()
  {
    if (OperatingSystem.IsWindows() && !File.Exists(@"\\.\pipe\docker_engine"))
    {
      Skip = "Docker is not running. Start Docker Desktop to run integration tests.";
    }
  }
}
