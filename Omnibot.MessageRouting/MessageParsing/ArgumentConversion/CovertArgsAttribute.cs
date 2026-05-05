#region

using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

/// <summary>
/// Is used to indicate the conversion for the command arguments.
/// </summary>
/// <param name="command">
/// The string literal of the command without the start symbol.
/// </param>
/// <param name="lifetime">
/// The lifetime of the converter in the di container.
/// Singleton by default.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CovertArgsAttribute(string command, ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public string Command => command;
    
    public ServiceLifetime Lifetime => lifetime;
}
