/// <summary>
/// Handles CPU performance monitoring and reporting via SSH connection.
/// </summary>
class ServerPerformanceCPU : ServerPerformanceBase
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
    /// Initializes a new instance of the <see cref="ServerPerformanceCPU"/> class.
    /// </summary>
    /// <param name="connection">The SSH connection to use for monitoring CPU performance.</param>
    public ServerPerformanceCPU(SshConnection connection, string os)
    {
        ServerConnection = connection;
        Metric = ServerPerformanceMetrics.CPU;
        SetOS(os);
    }

    /// <summary>
    /// Retrieves current CPU usage statistics including load average and top CPU-consuming processes.
    /// </summary>
    /// <returns>A string containing formatted CPU performance data.</returns>
    public override string CheckPerformance()
    {
        if (ServerConnection.Client == null)
        {
            return "SSH client not connected";
        }

        string commandString = OS switch
        {
            OperatingSystem.Linux   => "echo 'CPU Usage:' && top -b -n 1 | grep '%Cpu(s)' && " +
                                       "echo && echo 'Load Average:' && uptime | awk '{print $8, $9, $10, $11, $12}' && " +
                                       "echo && echo 'Top CPU Processes:' && " +
                                       "ps aux --sort=-%cpu | head -4 | awk 'NR==1{print $0} NR>1{printf \"%-8s %6s %6s %s\\n\", $1, $3, $4, $11}'",

            OperatingSystem.Windows => "echo 'CPU Usage:' && wmic cpu get loadpercentage /value && " +
                                       "echo && echo 'Top CPU Processes:' && " +
                                       "powershell -command \"Get-Process | Sort CPU -Descending | Select -First 5 Name,CPU\"",

            OperatingSystem.Mac     => "echo 'CPU Usage:' && top -l 1 | grep 'CPU usage' && " +
                                       "echo && echo 'Load Average:' && uptime && " +
                                       "echo && echo 'Top CPU Processes:' && " +
                                       "ps aux -r | head -5",

                                    // default to linux
            _                       => "echo 'CPU Usage:' && top -b -n 1 | grep '%Cpu(s)' && " +
                                       "echo && echo 'Load Average:' && uptime | awk '{print $8, $9, $10, $11, $12}' && " +
                                       "echo && echo 'Top CPU Processes:' && " +
                                       "ps aux --sort=-%cpu | head -4 | awk 'NR==1{print $0} NR>1{printf \"%-8s %6s %6s %s\\n\", $1, $3, $4, $11}'"
        };
        
        var command = ServerConnection.Client.RunCommand(commandString);
        return command.Result;
    }

    /// <summary>
    /// Checks the CPU usage for a specific process by its ID.
    /// </summary>
    /// <param name="processId">The process ID to check.</param>
    /// <returns>A string containing the CPU usage data for the specified process.</returns>
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