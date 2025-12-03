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
var connection = new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);

// Handle CLI keyboard interrupt
var cts = new CancellationTokenSource();
cli.StartKeyboardHandling(cts);

if (connection.Connect())
{
    // Main loop
    while (!cli.QuitApp)
    {
        // Poll server CPU performance
        var cpu = new ServerPerformanceCPU(connection);
        var cpuMetrics = cpu.CheckPerformance();
        Console.WriteLine(cpuMetrics);

        // Poll server disk performance
        var disk = new ServerPerformanceDisk(connection);
        var diskMetrics = disk.CheckPerformance();
        Console.WriteLine(diskMetrics);

        // Poll server memory performance
        var memory = new ServerPerformanceMemory(connection);
        var memoryMetrics = memory.CheckPerformance();
        Console.WriteLine(memoryMetrics);

        // Poll server network performance
        var network = new ServerPerformanceNetwork(connection);
        var networkMetrics = network.CheckPerformance();
        Console.WriteLine(networkMetrics);

        Thread.Sleep(1000);
    }
}
else
{
    Logger.Instance.LogInfo("Down time detected, informing admin.");
    Emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
}