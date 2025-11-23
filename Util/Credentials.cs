using System;
using System.IO;

public static class Credentials
{
    public static string? Host { get; private set; }
    public static string? User { get; private set; }
    public static string? Password { get; private set; }
    public static int TcpPort { get; private set; }

    // Load credentials from file
    public static void Load()
    {
        // Parse lines
        string credsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".creds", "ServerStatus.txt");
        string[] creds   = File.ReadAllLines(credsPath);

        Host     = creds[0];
        User     = creds[1];
        Password = creds[2];
        TcpPort  = int.Parse(creds[3]);
    }
}