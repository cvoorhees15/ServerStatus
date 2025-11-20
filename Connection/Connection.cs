using System;

abstract class Connection()
{
    // Manage any type of connection
    public abstract bool connect();

    public abstract bool disconnect();

    public abstract bool reconnect();

    // Store connection information
    public abstract string ConnectionType { get; set; }

    public abstract bool ConnectionStatus { get; set; }
}