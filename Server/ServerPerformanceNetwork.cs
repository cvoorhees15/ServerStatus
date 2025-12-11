/// <summary>
/// Handles network performance monitoring and reporting via SSH connection.
/// </summary>
class ServerPerformanceNetwork : ServerPerformanceBase
{
    // Override Fields
    private SshConnection? serverConnection;
    private ServerPerformanceMetrics metric;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerPerformanceNetwork"/> class.
    /// </summary>
    /// <param name="connection">The SSH connection to use for monitoring network performance.</param>
    public ServerPerformanceNetwork(SshConnection connection)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.Network;
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
        
        var command = ServerConnection.Client.RunCommand(
            "echo 'Network Interfaces:' && ip -s link show | head -10 && " +
            "echo && echo 'Active Connections:' && " +
            "ss -tulnp | head -6 && " +
            "echo && echo 'Network Traffic:' && " +
            "cat /proc/net/dev | head -4 | awk 'NR>2 {printf \"%-10s RX: %s TX: %s\\n\", $1, $2, $10}'"
        );
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
}