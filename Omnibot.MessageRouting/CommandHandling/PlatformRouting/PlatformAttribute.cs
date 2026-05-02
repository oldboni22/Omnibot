namespace Omnibot.MessageRouting.CommandHandling.PlatformRouting;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PlatformAttribute(string platform) : Attribute
{
    public string Platform => platform;
}
