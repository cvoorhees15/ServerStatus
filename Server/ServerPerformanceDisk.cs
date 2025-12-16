/// <summary>
/// Handles disk performance monitoring and reporting via SSH connection.
/// </summary>
class ServerPerformanceDisk : ServerPerformanceBase
{
    // Override Fields
    private SshConnection? serverConnection;
    private ServerPerformanceMetrics metric;
    private OperatingSystem os;

    // Properties
    protected override SshConnection ServerConnection
    {
        get { return serverConnection!; }
        set { serverConnection = value; }
    }

    protected override ServerPerformanceMetrics Metric
    {
        get { return metric; }
        set { metric = value; }
    }

    protected override OperatingSystem OS
    {
        get { return os; }
        set { os = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerPerformanceDisk"/> class.
    /// </summary>
    /// <param name="connection">The SSH connection to use for monitoring disk performance.</param>
    public ServerPerformanceDisk(SshConnection connection, string os)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.Disk;
        SetOS(os);
    }

    /// <summary>
    /// Retrieves current disk usage statistics including filesystem usage and I/O activity.
    /// </summary>
    /// <returns>A string containing formatted disk performance data.</returns>
    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }

        string commandString = OS switch
        {
            OperatingSystem.Linux   => "echo 'Disk Usage:' && df -h | head -6 && " +
                                       "echo && echo 'Disk I/O Activity:' && " +
                                       "iostat -x 1 1 | tail -n +4 | head -5 2>/dev/null || echo 'iostat not available - install sysstat package'",

            OperatingSystem.Windows => "echo 'Disk Usage:' && wmic logicaldisk get caption,size,freespace /value && " +
                                       "echo && echo 'Disk I/O Activity:' && " +
                                       "powershell -command \"Get-Counter '\\PhysicalDisk(*)\\Disk Read Bytes/sec', '\\PhysicalDisk(*)\\Disk Write Bytes/sec' -SampleInterval 1 -MaxSamples 1\"",

            OperatingSystem.Mac     => "echo 'Disk Usage:' && df -h && " +
                                       "echo && echo 'Disk I/O Activity:' && " +
                                       "iostat -d 1 1",

                                    // default to linux
            _                       => "echo 'Disk Usage:' && df -h | head -6 && " +
                                       "echo && echo 'Disk I/O Activity:' && " +
                                       "iostat -x 1 1 | tail -n +4 | head -5 2>/dev/null || echo 'iostat not available - install sysstat package'"
        };
        
        var command = ServerConnection.Client.RunCommand(commandString);
        return command.Result;
    }

    /// <summary>
    /// Checks the disk-related information for a specific process by its ID.
    /// </summary>
    /// <param name="processId">The process ID to check.</param>
    /// <returns>A string containing the process information.</returns>
    public override string CheckProcess(int processId)
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        var command = ServerConnection.Client.RunCommand($"ps -p {processId} -o pid,ppid,cmd");
        return command.Result;
    }
}