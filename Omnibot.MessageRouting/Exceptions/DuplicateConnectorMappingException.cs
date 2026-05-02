namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateConnectorMappingException(string connectorId) 
    : Exception($"Duplicate connector mapping for {connectorId}.");
