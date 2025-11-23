using System;
using System.Net.NetworkInformation;

class PingConnection : Connection
{
    // Override Fields
    private string connectionType = "Ping";
    private bool connectionStatus = false;

    // New Fields
    private string hostName = "";
    private int timeout = 5000; // 5 seconds default

    // Properties
    public override string ConnectionType
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

    public PingConnection(string host, int timeoutMs = 5000)
    {
        HostName = host;
        Timeout = timeoutMs;
    }

    public override bool connect()
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

    public override bool disconnect()
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

    public override bool reconnect()
    {
        // Ping reconnection logic
        // Disconnect, then connect
        if (!disconnect())
            return false;

        return connect();
    }
}