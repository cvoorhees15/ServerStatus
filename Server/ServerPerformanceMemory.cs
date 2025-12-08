class ServerPerformanceMemory : ServerPerformanceBase
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

    public ServerPerformanceMemory(SshConnection connection)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.Memory;
    }

    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        
        // Enhanced memory command that provides dynamic information similar to top's CPU output
        var command = ServerConnection.Client.RunCommand(
            "echo 'Memory Usage:' && free -h | head -2 && " +
            "echo && echo 'Memory Pressure:' && " +
            "cat /proc/meminfo | grep -E '^(MemAvailable|Buffers|Cached|SwapCached|Active|Inactive|Dirty|Writeback|Slab)' | head -5 && " +
            "echo && echo 'Top Memory Processes:' && " +
            "ps aux --sort=-%mem | head -4 | awk 'NR==1{print $0} NR>1{printf \"%-8s %6s %6s %s\\n\", $1, $3, $4, $11}'"
        );
        return command.Result;
    }

    public override string CheckProcess(int processId)
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        var command = ServerConnection.Client.RunCommand($"ps -p {processId} -o pid,ppid,cmd,%mem");
        return command.Result;
    }
}