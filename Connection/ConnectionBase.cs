using System;

/// <summary>
/// Provides a base class for different types of server connections.
/// </summary>
abstract class ConnectionBase()
{
    /// <summary>
    /// Defines the types of connections supported.
    /// </summary>
    protected enum ConnectionBaseTypes
    {
        SSH,
        TCP,
        Ping
    }

    // Store connection information
    protected abstract ConnectionBaseTypes ConnectionType { get; set; }

    public abstract bool ConnectionStatus { get; set; }

    /// <summary>
    /// Attempts to establish a connection to the server.
    /// </summary>
    /// <returns>True if the connection was successful, otherwise false.</returns>
    public abstract bool Connect();

    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    /// <returns>True if the disconnection was successful, otherwise false.</returns>
    public abstract bool Disconnect();

    /// <summary>
    /// Attempts to reconnect to the server.
    /// </summary>
    /// <returns>True if the reconnection was successful, otherwise false.</returns>
    public abstract bool Reconnect();
}