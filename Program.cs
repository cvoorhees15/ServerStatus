using System;
using System.IO;
using System.Linq;
using System.Threading;
using ServerStatus.Util;

// Initialization
Credentials.Load();

Emailer emailer = new Emailer(Credentials.SmtpHost!, Credentials.SmtpPort, Credentials.User!, Credentials.Password!);

Connection connection;

// Process CLI args
if (args.Contains("--tcp"))
{
    connection = new TCPConnection(Credentials.Host!, Credentials.TcpPort);
}
else if (args.Contains("--ssh"))
{
    connection = new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
}
else if (args.Contains("--ping"))
{
    connection = new PingConnection(Credentials.Host!);
}
else // default to ssh for now
{
    connection = new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
}

Logger.Instance.LogInfo("Enter 'q' to quit...");

// Used to interrupt/quit main thread while it's sleeping
bool shouldQuit = false;
var cts = new CancellationTokenSource();

// Start a background thread to listen for quit command
var inputThread = new Thread(() =>
{
    while (!shouldQuit)
    {
        // Handle keyboard input
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            if (char.ToLower(key.KeyChar) == 'q')
            {
                shouldQuit = true;
                cts.Cancel();
                Logger.Instance.LogInfo("Quitting...");
            }
        }
        else
        {
            Thread.Sleep(100);
        }
    }
});

inputThread.IsBackground = true;
inputThread.Start();

// Main loop
while (!shouldQuit)
{
    if (connection.connect())
    {
        cts.Token.WaitHandle.WaitOne(60000);
    }
    else
    {
        Logger.Instance.LogInfo("Down time detected, informing admin.");
        emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
    }

    connection.disconnect();
}