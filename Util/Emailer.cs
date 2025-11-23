using System.Net.Mail;

namespace ServerStatus.Util;

public class Emailer
{
    public string SmtpHost { get; private set; }
    public int    SmtpPort { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }

    public Emailer(string smtpHost, int smtpPort, string username, string password)
    {
        SmtpHost = smtpHost;
        SmtpPort = smtpPort;
        Username = username;
        Password = password;
    }

    public void SendEmail(string to, string subject, string body)
    {
        using var client = new SmtpClient(SmtpHost, SmtpPort)
        {
            Credentials = new System.Net.NetworkCredential(Username, Password),
            EnableSsl = true
        };

        var mail = new MailMessage(Username, to, subject, body);
        client.Send(mail);
    }
}