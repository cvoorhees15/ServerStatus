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

    public ServerPerformanceNetwork(SshConnection connection)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.Network;
    }

    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        
        // Enhanced network command with connection stats and traffic
        var command = ServerConnection.Client.RunCommand(
            "echo 'Network Interfaces:' && ip -s link show | head -10 && " +
            "echo && echo 'Active Connections:' && " +
            "ss -tulnp | head -6 && " +
            "echo && echo 'Network Traffic:' && " +
            "cat /proc/net/dev | head -4 | awk 'NR>2 {printf \"%-10s RX: %s TX: %s\\n\", $1, $2, $10}'"
        );
        return command.Result;
    }

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