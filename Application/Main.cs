using System;
using System.IO;
using System.Linq;
using System.Threading;
using ServerStatus.Util;

// Initializate utils
Credentials.Load();
Emailer.Load(Credentials.SmtpHost!, Credentials.SmtpPort, Credentials.User!, Credentials.Password!);

// Translate CLI args
var cli = new CommandLineManager();
var connection = cli.ParseArgs(args);

// Handle CLI keyboard interrupt
var cts = new CancellationTokenSource();
cli.StartKeyboardHandling(cts);

// Main loop
while (!cli.QuitApp)
{
    if (connection.Connect())
    {
        cts.Token.WaitHandle.WaitOne(60000);
    }
    else
    {
        Logger.Instance.LogInfo("Down time detected, informing admin.");
        Emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
    }

    connection.Disconnect();
}