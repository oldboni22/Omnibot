using System.Collections.Frozen;
using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.MessageRouting.CommandHandling.ConnectorRouting;

public sealed class ConnectorRoutingBuilder
{
    private readonly HashSet<string> _registeredCommands = [];
    
    private readonly HashSet<string> _registeredConnectors = [];
    
    private readonly List<string> _routedCommands = [];

    private readonly Dictionary<string, string> _connectorIdToPlatformKey = [];

    private bool _routeAll = false;
    
    internal ConnectorRoutingBuilder(){}

    public ConnectorRoutingBuilder RouteCommand(string command)
    {
        if (!_registeredCommands.Add(command))
        {
            throw new DuplicateCommandRoutingException(command);
        }
        
        _routedCommands.Add(command);
        return this;
    }
    
    public ConnectorRoutingBuilder RouteCommands(string[] commands)
    {
        foreach (var command in commands)
        {
            RouteCommand(command);
        }
        
        return this;
    }

    public ConnectorRoutingBuilder RouteAll()
    {
        _routeAll = true;
        return this;
    }
    
    public ConnectorRoutingBuilder MapConnector(string connectorId, string mapTo)
    {
        if (!_registeredConnectors.Add(connectorId))
        {
            throw new DuplicateConnectorMappingException(connectorId);
        }
        
        _connectorIdToPlatformKey[connectorId] =  mapTo;
        return this;
    }

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
