namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateCommandRoutingException(string command) 
    : Exception($"Duplicate routing registered for command {command}.");
