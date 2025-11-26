using System;
using System.Linq;
using System.Threading;
using ServerStatus.Util;

class CommandLineManager
{
    public CommandLineManager()
    {
        Console.WriteLine(@"
  ____                             ____  _        _             
 / ___|  ___ _ ____   _____ _ __  / ___|| |_ __ _| |_ _   _ ___ 
 \___ \ / _ \ '__\ \ / / _ \ '__| \___ \| __/ _` | __| | | / __|
  ___) |  __/ |   \ V /  __/ |     ___) | || (_| | |_| |_| \__ \
 |____/ \___|_|    \_/ \___|_|    |____/ \__\__,_|\__|\__,_|___/

");
    }

    public bool QuitApp { get; private set; } = false;

    public ConnectionBase ParseArgs(string[] args)
    {
        if (args.Contains("--tcp"))
        {
            return new TCPConnection(Credentials.Host!, Credentials.TcpPort);
        }
        else if (args.Contains("--ssh"))
        {
            return new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
        }
        else if (args.Contains("--ping"))
        {
            return new PingConnection(Credentials.Host!);
        }
        else // default to ssh for now
        {
            return new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
        }
    }

    public void StartKeyboardHandling(CancellationTokenSource cts)
    {
        Logger.Instance.LogInfo("Enter 'q' to quit...");

        var inputThread = new Thread(() =>
        {
            while (!QuitApp)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (char.ToLower(key.KeyChar) == 'q')
                    {
                        QuitApp = true;
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
    }
}