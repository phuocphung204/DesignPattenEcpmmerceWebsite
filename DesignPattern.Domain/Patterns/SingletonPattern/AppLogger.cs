using System.Globalization;

namespace DesignPattern.Domain.Patterns.SingletonPattern;

/// <summary>
/// Simple thread-safe Singleton logger that writes logs to a file.
/// </summary>
public sealed class AppLogger
{
  // Eager initialization of the singleton instance. This is thread-safe without needing locks.
  private static readonly AppLogger _instance = new();
  private static readonly object _fileLock = new();

  private readonly string _logFilePath;

  private AppLogger()
  {
    var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
    Directory.CreateDirectory(logDirectory);
    _logFilePath = Path.Combine(logDirectory, "singleton-log.txt");
  }

  public static AppLogger Instance => _instance;

  public void LogInfo(string message)
  {
    Write("INFO", message);
  }

  public void LogWarning(string message)
  {
    Write("WARN", message);
  }

  public void LogError(string message, Exception? exception = null)
  {
    var details = exception is null ? message : $"{message} | {exception.Message}";
    Write("ERROR", details);
  }

  private void Write(string level, string message)
  {
    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
    var line = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";

    lock (_fileLock)
    {
      File.AppendAllText(_logFilePath, line);
    }
  }
}
