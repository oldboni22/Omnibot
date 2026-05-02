using System.Collections.Frozen;
using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.MessageRouting.CommandHandling.PlatformRouting;

public sealed class PlatformRoutingBuilder
{
    private readonly HashSet<string> _registeredCommands = [];
    
    private readonly HashSet<string> _registeredConnectors = [];
    
    private readonly List<string> _routedCommands = [];

    private readonly Dictionary<string, string> _connectorIdToPlatformKey = [];

    private bool _routeAll = false;
    
    internal PlatformRoutingBuilder(){}

    public PlatformRoutingBuilder RouteCommand(string command)
    {
        if (!_registeredCommands.Add(command))
        {
            throw new DuplicateCommandRoutingException(command);
        }
        
        _routedCommands.Add(command);
        return this;
    }

    public PlatformRoutingBuilder RouteAll()
    {
        _routeAll = true;
        return this;
    }
    
    public PlatformRoutingBuilder MapConnector(string connectorId, string mapTo)
    {
        if (!_registeredConnectors.Add(connectorId))
        {
            throw new DuplicateConnectorMappingException(connectorId);
        }
        
        _connectorIdToPlatformKey[connectorId] =  mapTo;
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
