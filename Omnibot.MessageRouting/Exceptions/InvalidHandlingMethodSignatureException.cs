using System.Reflection;

namespace Omnibot.MessageRouting.Exceptions;

public sealed class InvalidHandlingMethodSignatureException(MethodInfo methodInfo) : Exception(
    $"Invalid handling method signature: {methodInfo.Name}. Message handling methods should not declare any parameters and return a Task.");
    