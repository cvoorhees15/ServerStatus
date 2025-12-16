using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
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

if (connection.Connect())
{
    // Create server performance objects
    var cpu = new ServerPerformanceCPU((SshConnection)connection, Credentials.OperatingSystem!);
    var disk = new ServerPerformanceDisk((SshConnection)connection, Credentials.OperatingSystem!);
    var memory = new ServerPerformanceMemory((SshConnection)connection, Credentials.OperatingSystem!);
    var network = new ServerPerformanceNetwork((SshConnection)connection, Credentials.OperatingSystem!);

    cli.StartDisplayMode();

    try
    {
        // Main loop with streaming display
        while (!cli.QuitApp)
        {
            // Poll server performance metrics
            var cpuMetrics = cpu.CheckPerformance();
            var diskMetrics = disk.CheckPerformance();
            var memoryMetrics = memory.CheckPerformance();
            var networkMetrics = network.CheckPerformance();

            // Display metrics in real-time dashboard
            cli.DisplayServerMetrics(cpuMetrics, memoryMetrics, diskMetrics, networkMetrics);

            Thread.Sleep(1000);
        }
    }
    finally
    {
        cli.EndDisplayMode();
        connection.Disconnect();
    }
}
else
{
    Logger.Instance.LogInfo("Down time detected, informing admin.");
    Emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
}