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

// Create CLI instance and TUI display
var cli = new CommandLineManager();
cli.StartDisplay();

// Handle CLI keyboard interrupt
var cts = new CancellationTokenSource();
cli.StartKeyboardHandling(cts);

// Create connnection instances
var sshConnection = new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
var pingConnection = new PingConnection(Credentials.Host!);

if (sshConnection.Connect())
{
    // Create server performance objects
    var cpu = new ServerPerformanceCPU(sshConnection, Credentials.OperatingSystem!);
    var disk = new ServerPerformanceDisk(sshConnection, Credentials.OperatingSystem!);
    var memory = new ServerPerformanceMemory(sshConnection, Credentials.OperatingSystem!);
    var network = new ServerPerformanceNetwork(sshConnection, pingConnection, Credentials.OperatingSystem!);


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

            // Poll server connectivity
            var latency = network.CheckConnectivity();

            // Display metrics in real-time dashboard
            cli.DisplayServerMetrics(cpuMetrics, memoryMetrics, diskMetrics, networkMetrics, latency);

            Thread.Sleep(1000);
        }
    }
    finally
    {
        cli.StopDisplay();
        sshConnection.Disconnect();
    }
}
else
{
    Logger.Instance.LogInfo("Down time detected, informing admin.");
    Emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
}