using System;
using System.IO;
using Renci.SshNet;

class SshConnection : Connection
{
    // Override Fields
    private string connectionType = "SSH";
    private bool connectionStatus = false;

    // New Fields
    private SshClient? client = null;
    private string hostName = "";
    private string userName = "";
    private string password = "";

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

    public override bool connect()
    {
        try
        {
            Logger.Instance.LogInfo($"Attempting connection to {hostName}...");

            // Store the client for later use
            Client = new SshClient(hostName, 22, userName, password);
            // Accept the host key

            // Optional: Set timeout
            // Client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);

            // Connect
            Client.Connect();
            Logger.Instance.LogInfo("Connected successfully!");

            // Test a command
            var testCommand = Client.RunCommand("systemctl status");
            Logger.Instance.LogInfo($"Test command output:\n {testCommand.Result}");

            if (testCommand.ExitStatus != 0)
            {
                Logger.Instance.LogError($"Test command failed: {testCommand.Error}");
            }

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

    public override bool disconnect()
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

    public override bool reconnect()
    {
        // SSH reconnection logic
        // Disconnect, then connect
        if (!disconnect())
            return false;

        return connect();
    }
}