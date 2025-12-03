using System;
using System.IO;
using Org.BouncyCastle.Tls;
using Renci.SshNet;

class SshConnection : ConnectionBase
{
    // Override Fields
    private ConnectionBaseTypes connectionType = ConnectionBaseTypes.SSH;
    private bool connectionStatus = false;

    // New Fields
    private SshClient? client = null;
    private string hostName = "";
    private string userName = "";
    private string password = "";

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

    public SshClient? Client
    {
        get { return client; }
        set { client = value; }
    }

    public string HostName
    {
        get { return hostName; }
        set { hostName = value; }
    }

    public string UserName
    {
        get { return userName; }
        set { userName = value; }
    }

    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    public SshConnection(string host, string user, string password)
    {
        HostName = host;
        UserName = user;
        Password = password;
    }

    public override bool Connect()
    {
        try
        {
            Logger.Instance.LogInfo($"Attempting {ConnectionType} connection to {HostName}...");

            // Store the client for later use
            Client = new SshClient(hostName, 22, userName, password);
            // Accept the host key

            // Optional: Set timeout
            // Client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);

            // Connect
            Client.Connect();
            Logger.Instance.LogInfo("Connected successfully!");

            // Test a command
            // var testCommand = Client.RunCommand("systemctl status");
            // Logger.Instance.LogInfo($"Test command output:\n {testCommand.Result}");

            connectionStatus = true;
        }
        catch (Renci.SshNet.Common.SshConnectionException ex)
        {
            Logger.Instance.LogError($"{ConnectionType} Connection error: {ex.Message}");
            connectionStatus = false;
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            Logger.Instance.LogError($"Network error: {ex.Message} (Error code: {ex.SocketErrorCode})");
            connectionStatus = false;
        }
        catch (Exception ex)
        {
            Logger.Instance.LogError($"Unknown error: {ex.Message}");
            connectionStatus = false;
        }

        return connectionStatus;
    }

    public override bool Disconnect()
    {
        // SSH disconnection logic
        if (Client != null && ConnectionStatus == true)
        {
            Client.Disconnect();
            Logger.Instance.LogInfo($"Disconnected from {HostName}");
            connectionStatus = false;
            return true;
        }
        else
        {
            Logger.Instance.LogInfo("No connection exists to disconnect from");
            return false;
        }
    }

    public override bool Reconnect()
    {
        // SSH reconnection logic
        // Disconnect, then connect
        if (!Disconnect())
            return false;

        return Connect();
    }
}