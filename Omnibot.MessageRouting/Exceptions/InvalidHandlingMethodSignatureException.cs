#region

using System.Reflection;

#endregion

namespace Omnibot.MessageRouting.Exceptions;

public sealed class InvalidHandlingMethodSignatureException(MethodInfo methodInfo) : Exception(
    $"Invalid handling method signature: {methodInfo.Name}. Message handling methods should declare no more than 1 parameter and return a Task.");
    