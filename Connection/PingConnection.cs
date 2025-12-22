using System;
using System.Net.NetworkInformation;

/// <summary>
/// Represents a ping-based connection to test server availability.
/// </summary>
class PingConnection : ConnectionBase
{
    // Override Fields
    private ConnectionBaseTypes connectionType = ConnectionBaseTypes.Ping;
    private bool connectionStatus = false;

    // New Fields
    private string hostName = "";
    private int timeout = 5000; // 5 seconds default
    private long latency = 0;

    // Properties
    protected override ConnectionBaseTypes ConnectionType
    {
        get { return connectionType; }
        set { connectionType = value; }
    }

    public override bool ConnectionStatus
    {
        get { return connectionStatus; }
        set { connectionStatus = value; }
    }

    public string HostName
    {
        get { return hostName; }
        set { hostName = value; }
    }

    public int Timeout
    {
        get { return timeout; }
        set { timeout = value; }
    }

    public long Latency
    {
        get { return latency; }
        set { latency = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PingConnection"/> class with the specified host and timeout.
    /// </summary>
    /// <param name="host">The hostname or IP address to ping.</param>
    /// <param name="timeoutMs">The timeout in milliseconds for the ping operation.</param>
    public PingConnection(string host, int timeoutMs = 5000)
    {
        HostName = host;
        Timeout = timeoutMs;
    }

    /// <summary>
    /// Attempts to ping the host to establish connectivity.
    /// </summary>
    /// <returns>True if the ping was successful, otherwise false.</returns>
    public override bool Connect()
    {
        try
        {
            Logger.Instance.LogInfo($"Pinging {hostName}...");

            using (Ping ping = new Ping())
            {
                PingReply reply = ping.Send(hostName, timeout);

                if (reply.Status == IPStatus.Success)
                {
                    Logger.Instance.LogInfo($"Ping successful. Round-trip time: {reply.RoundtripTime}ms");
                    Latency = reply.RoundtripTime;
                    connectionStatus = true;
                }
                else
                {
                    Logger.Instance.LogError($"Ping failed: {reply.Status}");
                    connectionStatus = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Ping error: {ex.Message}");
            connectionStatus = false;
        }

        return connectionStatus;
    }

    /// <summary>
    /// Disconnects the ping connection by updating the status.
    /// </summary>
    /// <returns>True if the disconnection was successful, otherwise false.</returns>
    public override bool Disconnect()
    {
        // Ping is stateless, so just update status
        if (connectionStatus)
        {
            Logger.Instance.LogInfo($"Ping connection to {hostName} disconnected");
            connectionStatus = false;
            return true;
        }
        else
        {
            Logger.Instance.LogInfo("No active ping connection to disconnect from");
            return false;
        }
    }

    /// <summary>
    /// Attempts to reconnect by disconnecting and then connecting again.
    /// </summary>
    /// <returns>True if the reconnection was successful, otherwise false.</returns>
    public override bool Reconnect()
    {
        // Ping reconnection logic
        // Disconnect, then connect
        if (!Disconnect())
            return false;

        return Connect();
    }
}