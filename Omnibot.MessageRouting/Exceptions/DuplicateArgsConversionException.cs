namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateArgsConversionException(Type type) 
    : Exception($"Duplicate argument conversion registered for type {type.Name}.");
