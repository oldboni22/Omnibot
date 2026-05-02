namespace Omnibot.MessageRouting.Exceptions;

public sealed class ConnectorRoutingNotConfiguredException(string connectorId) 
    : Exception($"Routing for connector {connectorId} is not configured.");
