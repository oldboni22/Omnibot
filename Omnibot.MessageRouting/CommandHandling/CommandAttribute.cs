namespace Omnibot.MessageRouting.CommandHandling;

/// <summary>
/// Is used to indicate which command a controller method should handle.
/// </summary>
/// <param name="command">
/// The string literal of the command without the start symbol.
/// </param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CommandAttribute(string command) : Attribute
{
    public string Command => command;
}
