/// <summary>
/// Handles network performance monitoring and reporting via SSH connection.
/// </summary>
class ServerPerformanceNetwork : ServerPerformanceBase
{
    // Override Fields
    private SshConnection? serverConnection;
    private ServerPerformanceMetrics metric;
    private OperatingSystem os;

    // New fields
    private PingConnection? pingConnection;

    // Properties
    protected override SshConnection ServerConnection
    {
        get { return serverConnection!; }
        set { serverConnection = value; }
    }

    protected PingConnection Pinger
    {
        get { return pingConnection!; }
        set { pingConnection = value; }
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
    /// Initializes a new instance of the <see cref="ServerPerformanceNetwork"/> class.
    /// </summary>
    /// <param name="ssh">The SSH connection to use for monitoring network performance.</param>
    /// <param name="ping">The ping connection to use for monitoring server connectivity.</param>
    /// <param name="os">The operating system of the server.</param>
    public ServerPerformanceNetwork(SshConnection ssh, PingConnection ping, string os)
    {
        ServerConnection = ssh;
        Pinger = ping;
        Metric = ServerPerformanceMetrics.Network;
        SetOS(os);
    }

    /// <summary>
    /// Retrieves current network statistics including interface status, active connections, and traffic data.
    /// </summary>
    /// <returns>A string containing formatted network performance data.</returns>
    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }

        string commandString = OS switch
        {
            OperatingSystem.Linux   => "echo 'Network Interfaces:' && ip -s link show | head -10 && " +
                                       "echo && echo 'Active Connections:' && " +
                                       "ss -tulnp | head -6 && " +
                                       "echo && echo 'Network Traffic:' && " +
                                       "cat /proc/net/dev | head -4 | awk 'NR>2 {printf \"%-10s RX: %s TX: %s\\n\", $1, $2, $10}'",

            OperatingSystem.Windows => "echo 'Network Interfaces:' && ipconfig /all && " +
                                       "echo && echo 'Active Connections:' && " +
                                       "netstat -an | findstr LISTENING | head -6 && " +
                                       "echo && echo 'Network Traffic:' && " +
                                       "powershell -command \"Get-NetAdapterStatistics | Select Name,ReceivedBytes,SentBytes\"",

            OperatingSystem.Mac     => "echo 'Network Interfaces:' && ifconfig && " +
                                       "echo && echo 'Active Connections:' && " +
                                       "netstat -tulnp | head -6 && " +
                                       "echo && echo 'Network Traffic:' && " +
                                       "netstat -i",

                                    // default to linux
            _                       => "echo 'Network Interfaces:' && ip -s link show | head -10 && " +
                                       "echo && echo 'Active Connections:' && " +
                                       "ss -tulnp | head -6 && " +
                                       "echo && echo 'Network Traffic:' && " +
                                       "cat /proc/net/dev | head -4 | awk 'NR>2 {printf \"%-10s RX: %s TX: %s\\n\", $1, $2, $10}'"
        };
        
        var command = ServerConnection.Client.RunCommand(commandString);
        return command.Result;
    }

    /// <summary>
    /// Checks the network connections for a specific process by its ID.
    /// </summary>
    /// <param name="processId">The process ID to check.</param>
    /// <returns>A string containing the network connection data for the specified process.</returns>
    public override string CheckProcess(int processId)
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        var command = ServerConnection.Client.RunCommand($"ss -p | grep {processId}");
        return command.Result;
    }

    /// <summary>
    /// Check the latency of the server connection.
    /// </summary>
    /// <returns>Server connection latency in ms</returns>
    public long CheckConnectivity()
    {
        if (Pinger.Connect())
        {
            return Pinger.Latency;
        }

        return -1;
    }
}