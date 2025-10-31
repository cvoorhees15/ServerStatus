using System;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;

class SshConnection : Connection
{
    // Override Fields
    private string     connectionType   = "SSH";
    private bool       connectionStatus = false;

    // New Fields
    private SshClient? client           = null;
    private string     hostName         = "";
    private string     userName         = "";
    private string     password         = "";

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

    public SshConnection (string host, string user, string password)
    {
        HostName = host;
        UserName = user;
        Password = password;
    }

    public override bool connect()
    {
        try // SSH connection logic
        {
            Logger.LogInfo($"Attempting connection to {hostName}...");
            using (var newClient = new SshClient(hostName, 22, userName, password))
            {
                Client = newClient;
                connectionStatus = true;

                // Set timeout
                // client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30);

                // Connect
                newClient.Connect();
                Logger.LogInfo("Connected successfully!");
                
                // Test a command
                var testCommand = newClient.RunCommand("hostname && uptime");
                Logger.LogInfo($"\nTest command output:\n{testCommand.Result}");
                
                if (testCommand.ExitStatus != 0)
                {
                    Logger.LogError($"Test command failed: {testCommand.Error}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Connection failed: {ex.Message}");
            connectionStatus = false;
        }
        return connectionStatus;
    }

    public override bool disconnect()
    {
        // SSH disconnection logic
        connectionStatus = false;
        return true;
    }

    public override bool reconnect()
    {
        // SSH reconnection logic
        // Disconnect, then connect
        disconnect();
        return connect();
    }
}