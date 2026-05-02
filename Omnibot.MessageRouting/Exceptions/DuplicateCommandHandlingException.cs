namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateCommandHandlingException(string command) 
    : Exception($"Duplicate handling registered for {command}.");
