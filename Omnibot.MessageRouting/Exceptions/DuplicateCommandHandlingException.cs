namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateCommandHandlingException(string command) : Exception($"The command {command} is already registered.");
