namespace Omnibot.MessageRouting.CommandHandling.ConnectorRouting;

[AttributeUsage(AttributeTargets.Class, Inherited =  false, AllowMultiple = true)]
public sealed class RouteAttribute(string platform) : Attribute
{
    public string Platform => platform;
}
