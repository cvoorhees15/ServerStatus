using System;

/// <summary>
/// Provides a thread-safe singleton logger for outputting messages to the console.
/// </summary>
public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new object();

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
    /// Logs a message to the console with a timestamp.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now}] {message}\n");
    }

    /// <summary>
    /// Logs an informational message to the console.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    public void LogInfo(string message)
    {
        Log($"INFO: {message}");
    }

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public void LogError(string message)
    {
        Log($"ERROR: {message}");
    }
}