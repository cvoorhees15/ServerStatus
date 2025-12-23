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

    /// <summary>
    /// Defines the type of operating system the server is running.
    /// </summary>
    protected enum OperatingSystem
    {
        Linux,
        Windows,
        Mac
    }

    protected abstract OperatingSystem OS { get; set; }

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

    /// <summary>
    /// Sets the operating system based on the provided OS string.
    /// </summary>
    /// <param name="osString">The operating system string read from the credentials file</param>
    /// <returns>An OperatingSystem enum</returns>
    protected void SetOS(string osString)
    {
        string lowerCaseOsString = osString.ToLower();

        this.OS = lowerCaseOsString switch
        {
            "linux"   => OperatingSystem.Linux,
            "windows" => OperatingSystem.Windows,
            "macos"   => OperatingSystem.Mac,
            _         => OperatingSystem.Linux // default to linux for now
        };
    }
}