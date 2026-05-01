namespace Omnibot.MessageRouting.CommandHandling;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandAttribute(string command) : Attribute
{
    public string Command => command;
}
