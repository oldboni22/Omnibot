#region

using System.Collections.Frozen;
using Omnibot.MessageRouting.Exceptions;

#endregion

namespace Omnibot.MessageRouting.CommandHandling.ConnectorRouting;

public sealed class ConnectorRoutingBuilder
{
    private readonly HashSet<string> _registeredCommands = [];
    
    private readonly HashSet<string> _registeredConnectors = [];
    
    private readonly List<string> _routedCommands = [];

    private readonly Dictionary<string, string> _connectorIdToPlatformKey = [];

    private bool _routeAll = false;
    
    internal ConnectorRoutingBuilder(){}

    /// <summary>
    /// Enables connector routing for the specified command.
    /// </summary>
    /// <param name="command">
    /// The string literal of the command without the start symbol.
    /// </param>
    /// <exception cref="DuplicateCommandRoutingException">
    /// Is thrown when the command is already registered.
    /// </exception>
    public ConnectorRoutingBuilder RouteCommand(string command)
    {
        if (!_registeredCommands.Add(command))
        {
            throw new DuplicateCommandRoutingException(command);
        }
        
        _routedCommands.Add(command);
        return this;
    }
    
    /// <summary>
    /// Enables connector routing for the specified command.
    /// </summary>
    /// <param name="commands">
    /// The array of string literals of the commands without the start symbol.</param>
    /// <exception cref="DuplicateCommandRoutingException">
    /// Is thrown when a command is already registered.
    /// </exception>
    public ConnectorRoutingBuilder RouteCommands(string[] commands)
    {
        foreach (var command in commands)
        {
            RouteCommand(command);
        }
        
        return this;
    }

    /// <summary>
    /// Enables connector routing for all the commands.
    /// </summary>
    public ConnectorRoutingBuilder RouteAll()
    {
        _routeAll = true;
        return this;
    }
    
    /// <summary>
    /// Configures mapping for the specified connector identifier.
    /// </summary>
    /// <param name="connectorId">
    /// The connector identifier.
    /// </param>
    /// <param name="mapTo">
    /// The string literal of the route to map to the connector id.
    /// </param>
    /// <exception cref="DuplicateConnectorMappingException">
    /// Is thrown when the connector is already registered.
    /// </exception>
    public ConnectorRoutingBuilder MapConnector(string connectorId, string mapTo)
    {
        if (!_registeredConnectors.Add(connectorId))
        {
            throw new DuplicateConnectorMappingException(connectorId);
        }
        
        _connectorIdToPlatformKey[connectorId] =  mapTo;
        return this;
    }

    /// <summary>
    /// Configures mapping for the specified connector identifier.
    /// </summary>
    /// <param name="mappings">
    /// The mappings.
    /// The first value in a pair represents the connector identifier.
    /// The second value in a pair represents the string literal of the route to map to the connector id.
    /// </param>
    /// <exception cref="DuplicateConnectorMappingException">
    /// Is thrown when a connector is already registered.
    /// </exception>
    public ConnectorRoutingBuilder MapConnectors(Dictionary<string, string> mappings)
    {
        foreach (var (connectorId, mapTo) in mappings)
        {
            MapConnector(connectorId, mapTo);
        }
    
        return this;
    }

    internal void Deconstruct(
        out FrozenSet<string> routedCommands,
        out FrozenDictionary<string, string> connectorIdToPlatformKey,
        out bool routeALl)
    {
        routedCommands = _routedCommands.ToFrozenSet();
        connectorIdToPlatformKey = _connectorIdToPlatformKey.ToFrozenDictionary();
        routeALl = _routeAll;
    }
}
