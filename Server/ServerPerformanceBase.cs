using System;
using Renci.SshNet;

abstract class ServerPerformanceBase
{
    protected enum ServerPerformanceMetrics
    {
        CPU,
        Memory,
        Disk,
        Network
    }

    // Store system information
    protected abstract ServerPerformanceMetrics Metric { get; set; }

    protected abstract SshConnection ServerConnection { get; set; }

    // Execute commands to aquire system performance metrics
    public abstract string CheckPerformance();

    public abstract string CheckProcess(int processId);
}