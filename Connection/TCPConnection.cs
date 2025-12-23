using System.Net.Sockets;

/// <summary>
/// Represents a TCP connection to a server for basic connectivity testing.
/// </summary>
class TCPConnection : ConnectionBase
{
    // Override Fields
    private ConnectionBaseTypes connectionType = ConnectionBaseTypes.TCP;
    private bool connectionStatus = false;

    // New Fields
    private TcpClient? client = null;
    private string hostName = "";
    private int port = 0;

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

    public TcpClient? Client
    {
        get { return client; }
        set { client = value; }
    }

    public string HostName
    {
        get { return hostName; }
        set { hostName = value; }
    }

    public int Port
    {
        get { return port; }
        set { port = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TCPConnection"/> class with the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the server.</param>
    /// <param name="port">The port number for the TCP connection.</param>
    public TCPConnection(string host, int port)
    {
        HostName = host;
        Port = port;
    }

    /// <summary>
    /// Attempts to establish a TCP connection to the server.
    /// </summary>
    /// <returns>True if the connection was successful, otherwise false.</returns>
    public override bool Connect()
    {
        try
        {
            Logger.Instance.LogInfo($"Attempting TCP connection to {hostName}:{port}...");

            // Create and connect the client
            Client = new TcpClient();
            Client.Connect(hostName, port);
            Logger.Instance.LogInfo("Connected successfully!");

            // Test the connection by checking if it's connected
            if (Client.Connected)
            {
                Logger.Instance.LogInfo("Connection test passed.");
            }
            else
            {
                Logger.Instance.LogError("Connection test failed.");
                connectionStatus = false;
                return false;
            }

            connectionStatus = true;
        }
        catch (SocketException ex)
        {
            Logger.Instance.LogError($"{ConnectionType} Connection error: {ex.Message} (Error code: {ex.SocketErrorCode})");
            connectionStatus = false;
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Unknown error: {ex.Message}");
            connectionStatus = false;
        }

        return connectionStatus;
    }

    /// <summary>
    /// Disconnects the TCP connection from the server.
    /// </summary>
    /// <returns>True if the disconnection was successful, otherwise false.</returns>
    public override bool Disconnect()
    {
        // TCP disconnection logic
        if (Client != null && ConnectionStatus == true)
        {
            Client.Close();
            Logger.Instance.LogInfo($"Disconnected from {HostName}:{Port}");
            connectionStatus = false;
            return true;
        }
        else
        {
            Logger.Instance.LogInfo("No connection exists to disconnect from");
            return false;
        }
    }

    /// <summary>
    /// Attempts to reconnect to the server by disconnecting and then connecting again.
    /// </summary>
    /// <returns>True if the reconnection was successful, otherwise false.</returns>
    public override bool Reconnect()
    {
        // TCP reconnection logic
        // Disconnect, then connect
        if (!Disconnect())
            return false;

        return Connect();
    }
}