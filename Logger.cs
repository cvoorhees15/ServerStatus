using System;

public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new object();

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

    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    public void LogInfo(string message)
    {
        Log($"INFO: {message}");
    }

    public void LogError(string message)
    {
        Log($"ERROR: {message}");
    }
}