class ServerPerformanceDisk : ServerPerformanceBase
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

    public ServerPerformanceDisk(SshConnection connection)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.Disk;
    }

    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        
        var command = ServerConnection.Client.RunCommand(
            "echo 'Disk Usage:' && df -h | head -6 && " +
            "echo && echo 'Disk I/O Activity:' && " +
            "iostat -x 1 1 | tail -n +4 | head -5 2>/dev/null || echo 'iostat not available - install sysstat package'"
        );
        return command.Result;
    }

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