using System;
using System.IO;

/// <summary>
/// Provides static access to application credentials loaded from a JSON configuration file.
/// </summary>
public static class Credentials
{
    public static string? Host            { get; private set; }
    public static string? User            { get; private set; }
    public static string? Password        { get; private set; }
    public static int     TcpPort         { get; private set; }
    public static string? SmtpHost        { get; private set; }
    public static int     SmtpPort        { get; private set; }
    public static string? AdminEmail      { get; private set; }
    public static string? OperatingSystem { get; private set; }

    /// <summary>
    /// Loads credentials from the ServerStatus.json file located in the user's .creds directory.
    /// </summary>
    public static void Load()
    {
        string credsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".creds", "ServerStatus.json");
        string jsonText = File.ReadAllText(credsPath);
        var creds = Newtonsoft.Json.Linq.JObject.Parse(jsonText);

        // SSH
        Host            = creds["Host"]?.ToString() ?? "";
        User            = creds["User"]?.ToString() ?? "";
        Password        = creds["Password"]?.ToString() ?? "";

        // TCP
        TcpPort         = creds["TcpPort"] != null ? (int)creds["TcpPort"]! : 0;

        // Email
        SmtpHost        = creds["SmtpHost"]?.ToString() ?? "";
        SmtpPort        = creds["SmtpPort"] != null ? (int)creds["SmtpPort"]! : 0;
        AdminEmail      = creds["AdminEmail"]?.ToString() ?? "";
        
        // OS
        OperatingSystem = creds["OperatingSystem"]?.ToString() ?? "";
    }
}