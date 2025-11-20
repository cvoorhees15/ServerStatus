using System;
using System.Net.Sockets;

class TCPConnection : Connection
{
    // Override Fields
    private string connectionType = "TCP";
    private bool connectionStatus = false;

    // New Fields
    private TcpClient? client = null;
    private string hostName = "";
    private int port = 0;

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

    public TCPConnection(string host, int port)
    {
        HostName = host;
        Port = port;
    }

    public override bool connect()
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

    public override bool disconnect()
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

    public override bool reconnect()
    {
        // TCP reconnection logic
        // Disconnect, then connect
        if (!disconnect())
            return false;

        return connect();
    }
}