/// <summary>
/// Represents a single log entry with timestamp, level, and message.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the timestamp when the log entry was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the severity level of the log entry.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the log message content.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="timestamp">The timestamp of the log entry.</param>
    /// <param name="level">The severity level of the log entry.</param>
    /// <param name="message">The log message content.</param>
    public LogEntry(DateTime timestamp, LogLevel level, string message)
    {
        Timestamp = timestamp;
        Level = level;
        Message = message;
    }

    /// <summary>
    /// Formats the log entry for display in the TUI.
    /// </summary>
    /// <returns>Formatted string: "[HH:mm:ss] LEVEL: message"</returns>
    public string ToDisplayString()
    {
        var levelStr = Level switch
        {
            LogLevel.Info => "INFO",
            LogLevel.Error => "ERROR",
            LogLevel.Warning => "WARN",
            _ => "LOG"
        };

        return $"[{Timestamp:HH:mm:ss}] {levelStr}: {Message}";
    }
}

/// <summary>
/// Defines the severity level of a log entry.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message.
    /// </summary>
    Error
}