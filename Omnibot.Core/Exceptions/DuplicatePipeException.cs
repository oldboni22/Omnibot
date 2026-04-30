namespace Omnibot.Core.Exceptions;

public sealed class DuplicatePipeException(Type type) : Exception($"The pipe of type {type} was already registered.");
