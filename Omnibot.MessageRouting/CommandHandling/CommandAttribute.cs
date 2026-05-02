namespace Omnibot.MessageRouting.CommandHandling;

[AttributeUsage(AttributeTargets.Method, Inherited =  false)]
public sealed class CommandAttribute(string command) : Attribute
{
    public string Command => command;
}
