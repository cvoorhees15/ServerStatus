using System;

abstract class ConnectionBase()
{
    protected enum ConnectionBaseTypes
    {
        SSH,
        TCP,
        Ping
    }

    // Store connection information
    protected abstract ConnectionBaseTypes ConnectionType { get; set; }

    public abstract bool ConnectionStatus { get; set; }

    // Manage any type of connection
    public abstract bool Connect();

    public abstract bool Disconnect();

    public abstract bool Reconnect();
}