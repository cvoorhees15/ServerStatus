using System.Net.Mail;

namespace ServerStatus.Util;

public static class Emailer
{
    public static string? SmtpHost { get; private set; }
    public static int     SmtpPort { get; private set; }
    public static string? Username { get; private set; }
    public static string? Password { get; private set; }

    public static void Load(string smtpHost, int smtpPort, string username, string password)
    {
        SmtpHost = smtpHost;
        SmtpPort = smtpPort;
        Username = username;
        Password = password;
    }

    public static void SendEmail(string to, string subject, string body)
    {
        using var client = new SmtpClient(SmtpHost, SmtpPort)
        {
            Credentials = new System.Net.NetworkCredential(Username, Password),
            EnableSsl = true
        };

        if (Username != null)
        {
            var mail = new MailMessage(Username, to, subject, body);
            client.Send(mail);
        }
    }
}