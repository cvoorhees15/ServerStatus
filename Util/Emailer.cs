using System.Net.Mail;

namespace ServerStatus.Util;

/// <summary>
/// Provides static methods for sending email notifications using SMTP.
/// </summary>
public static class Emailer
{
    public static string? SmtpHost { get; private set; }
    public static int     SmtpPort { get; private set; }
    public static string? Username { get; private set; }
    public static string? Password { get; private set; }

    /// <summary>
    /// Initializes the Emailer with SMTP configuration.
    /// </summary>
    /// <param name="smtpHost">The SMTP server hostname.</param>
    /// <param name="smtpPort">The SMTP server port number.</param>
    /// <param name="username">The username for SMTP authentication.</param>
    /// <param name="password">The password for SMTP authentication.</param>
    public static void Load(string smtpHost, int smtpPort, string username, string password)
    {
        SmtpHost = smtpHost;
        SmtpPort = smtpPort;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Sends an email message using the configured SMTP settings.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The email message body.</param>
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