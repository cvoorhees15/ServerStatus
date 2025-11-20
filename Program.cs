using System;
using System.Threading;

SshConnection ssh = new SshConnection("voorhees-server1", "caleb", "Fcalax#17");

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
    ssh.connect();
    cts.Token.WaitHandle.WaitOne(60000);
    ssh.disconnect();
}