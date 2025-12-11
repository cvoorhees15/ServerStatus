using System;
using Renci.SshNet;

/// <summary>
/// Provides a base class for retrieving various server performance metrics via SSH.
/// </summary>
abstract class ServerPerformanceBase
{
    /// <summary>
    /// Defines the types of performance metrics that can be monitored.
    /// </summary>
    protected enum ServerPerformanceMetrics
    {
        CPU,
        Memory,
        Disk,
        Network
    }

    protected abstract ServerPerformanceMetrics Metric { get; set; }

    protected abstract SshConnection ServerConnection { get; set; }

    /// <summary>
    /// Executes commands to retrieve the current performance metrics for the specific metric type.
    /// </summary>
    /// <returns>A string containing the formatted performance data.</returns>
    public abstract string CheckPerformance();

    /// <summary>
    /// Checks the performance metrics for a specific process by its ID.
    /// </summary>
    /// <param name="processId">The process ID to check.</param>
    /// <returns>A string containing the process performance data.</returns>
    public abstract string CheckProcess(int processId);
}