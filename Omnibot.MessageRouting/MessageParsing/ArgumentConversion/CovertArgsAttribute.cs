using Microsoft.Extensions.DependencyInjection;

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CovertArgsAttribute(string command, ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public string Command => command;
    
    public ServiceLifetime Lifetime => lifetime;
}
