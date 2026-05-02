namespace Omnibot.MessageRouting.CommandHandling.ConnectorRouting;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute(string platform) : Attribute
{
    public string Platform => platform;
}
