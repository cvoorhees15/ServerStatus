class ServerPerformanceCPU : ServerPerformanceBase
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

    public ServerPerformanceCPU(SshConnection connection)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.CPU;
    }

    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        
        var command = ServerConnection.Client.RunCommand(
            "echo 'CPU Usage:' && top -b -n 1 | grep '%Cpu(s)' && " +
            "echo && echo 'Load Average:' && uptime | awk '{print $8, $9, $10, $11, $12}' && " +
            "echo && echo 'Top CPU Processes:' && " +
            "ps aux --sort=-%cpu | head -4 | awk 'NR==1{print $0} NR>1{printf \"%-8s %6s %6s %s\\n\", $1, $3, $4, $11}'"
        );
        return command.Result;
    }

    public override string CheckProcess(int processId)
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }
        var command = ServerConnection.Client.RunCommand($"ps -p {processId} -o pid,ppid,cmd,%cpu");
        return command.Result;
    }
}