using System;
using System.IO;

public static class Credentials
{
    public static string? Host       { get; private set; }
    public static string? User       { get; private set; }
    public static string? Password   { get; private set; }
    public static int     TcpPort    { get; private set; }
    public static string? SmtpHost   { get; private set; }
    public static int     SmtpPort   { get; private set; }
    public static string? AdminEmail { get; private set; }

    // Load credentials from file
    public static void Load()
    {
        // Parse lines
        string credsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".creds", "ServerStatus.txt");
        string[] creds   = File.ReadAllLines(credsPath);

        // SSH
        Host       = creds[0];
        User       = creds[1];
        Password   = creds[2];

        // TCP
        TcpPort    = int.Parse(creds[3]);

        // Email
        SmtpHost   = creds[4];
        SmtpPort   = int.Parse(creds[5]);
        AdminEmail = creds[6];
    }
}