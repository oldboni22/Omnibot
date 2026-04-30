namespace Omnibot.Core.Exceptions;

public class DuplicateConnectorException(Type type) : Exception($"The connector of type {type} was already registered.");
