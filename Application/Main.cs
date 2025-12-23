
using ServerStatus.Util;

// Load utils
Credentials.Load();
Emailer.Load(Credentials.SmtpHost!, Credentials.SmtpPort, Credentials.User!, Credentials.Password!);

// Create CLI and TUI display
var cli = new CommandLineManager();
cli.StartDisplay();

// Handle CLI keyboard interrupt
var cts = new CancellationTokenSource();
cli.StartKeyboardHandling(cts);

// Create connnections
var sshConnection = new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
var pingConnection = new PingConnection(Credentials.Host!);

if (sshConnection.Connect())
{
    // Create server metrics
    var cpu = new ServerPerformanceCPU(sshConnection, Credentials.OperatingSystem!);
    var disk = new ServerPerformanceDisk(sshConnection, Credentials.OperatingSystem!);
    var memory = new ServerPerformanceMemory(sshConnection, Credentials.OperatingSystem!);
    var network = new ServerPerformanceNetwork(sshConnection, pingConnection, Credentials.OperatingSystem!);

    try
    {
        // Main loop
        while (!cli.QuitApp)
        {
            // Poll server performance
            var cpuMetrics = cpu.CheckPerformance();
            var diskMetrics = disk.CheckPerformance();
            var memoryMetrics = memory.CheckPerformance();
            var networkMetrics = network.CheckPerformance();

            // Poll server connectivity
            var latency = network.CheckConnectivity();

            // Refresh metrics dashboard
            cli.DisplayServerMetrics(cpuMetrics, memoryMetrics, diskMetrics, networkMetrics, latency);

            // Once per second
            Thread.Sleep(1000);
        }
    }
    finally
    {
        // Quit
        cli.StopDisplay();
        sshConnection.Disconnect();
    }
}
else
{
    // Connection failed, inform admin
    Logger.Instance.LogInfo("Down time detected, informing admin.");
    Emailer.SendEmail(Credentials.AdminEmail!, "Server Down", "The server is down.");
}