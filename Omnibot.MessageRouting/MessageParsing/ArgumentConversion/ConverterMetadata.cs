using Microsoft.Extensions.DependencyInjection;

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

internal sealed record ConverterMetadata(string Command, Type ServiceType, Type ImplementationType, ServiceLifetime ServiceLifetime);
