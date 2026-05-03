#region

using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

internal sealed record ConverterMetadata(string Command, Type ServiceType, Type ImplementationType, ServiceLifetime ServiceLifetime);
