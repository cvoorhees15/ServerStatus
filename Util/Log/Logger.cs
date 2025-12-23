using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides a thread-safe singleton logger for outputting messages to the console.
/// </summary>
public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new object();

    // Circular buffer for log entries
    private readonly List<LogEntry> _logBuffer = new List<LogEntry>();
    private const int MAX_BUFFER_SIZE = 100;

    // Display mode flag - when true, suppresses console output
    private bool _displayModeActive = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    private Logger() { }

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Enables display mode, which suppresses console output and only buffers logs.
    /// </summary>
    public void EnableDisplayMode()
    {
        lock (_lock)
        {
            _displayModeActive = true;
        }
    }

    /// <summary>
    /// Disables display mode, returning to normal console output behavior.
    /// </summary>
    public void DisableDisplayMode()
    {
        lock (_lock)
        {
            _displayModeActive = false;
        }
    }

    /// <summary>
    /// Gets the most recent log entries from the buffer.
    /// </summary>
    /// <param name="count">Maximum number of entries to retrieve.</param>
    /// <returns>List of recent log entries, newest first.</returns>
    public List<LogEntry> GetRecentLogs(int count)
    {
        lock (_lock)
        {
            return _logBuffer
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .ToList();
        }
    }

    /// <summary>
    /// Adds a log entry to the circular buffer.
    /// </summary>
    /// <param name="level">The severity level of the log entry.</param>
    /// <param name="message">The log message content.</param>
    private void AddToBuffer(LogLevel level, string message)
    {
        lock (_lock)
        {
            var entry = new LogEntry(DateTime.Now, level, message);
            _logBuffer.Add(entry);

            // Maintain circular buffer size
            if (_logBuffer.Count > MAX_BUFFER_SIZE)
            {
                _logBuffer.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Core logging method that handles buffering and console output.
    /// </summary>
    /// <param name="level">The severity level of the log entry.</param>
    /// <param name="message">The log message content.</param>
    private void LogWithLevel(LogLevel level, string message)
    {
        // Always add to buffer for history
        AddToBuffer(level, message);

        // Only write to console if NOT in display mode
        lock (_lock)
        {
            if (!_displayModeActive)
            {
                Console.WriteLine($"[{DateTime.Now}] {message}\n");
            }
        }
    }

    /// <summary>
    /// Logs a message to the console with a timestamp.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        LogWithLevel(LogLevel.Info, message);
    }

    /// <summary>
    /// Logs an informational message to the console.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    public void LogInfo(string message)
    {
        LogWithLevel(LogLevel.Info, $"INFO: {message}");
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public void LogError(string message)
    {
        LogWithLevel(LogLevel.Error, $"ERROR: {message}");
    }
}