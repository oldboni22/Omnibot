namespace Omnibot.MessageRouting.Exceptions;

public sealed class DuplicateAuthPolicyException(string policy) 
    : Exception($"Duplicate auth policy registered for {policy}.");
    