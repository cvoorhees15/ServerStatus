using System;

abstract class ConnectionBase()
{
    // Store connection information
    public abstract string ConnectionType { get; set; }

    public abstract bool ConnectionStatus { get; set; }

    // Manage any type of connection
    public abstract bool connect();

    public abstract bool disconnect();

    public abstract bool reconnect();
}