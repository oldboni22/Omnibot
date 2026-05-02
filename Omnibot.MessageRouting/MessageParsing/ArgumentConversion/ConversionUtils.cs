using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Omnibot.MessageRouting.CommandHandling;
using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public static class ConversionUtils
{
    extension(Assembly[] assemblies)
    {
        internal ConverterMetadata[] GetValidConvertersMetadata()
        {
            return assemblies
                .AsParallel()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t is { IsAbstract: false, IsInterface: false } && t.IsConverter())
                .Where(t => Attribute.IsDefined(t, typeof(CovertArgsAttribute)))
                .SelectMany(GetConverterMetadata)
                .ToArray();
        }
    }

    extension(Type type)
    {
        private bool IsConverter() => type.GetConverterBase() is not null;
        
        private IEnumerable<ConverterMetadata> GetConverterMetadata()
        {
            var serviceType = type.GetConverterBase()!;
            
            var convertAttributes = Attribute.GetCustomAttributes(type, typeof(CovertArgsAttribute)) as CovertArgsAttribute[];

            foreach (var convertAttribute in convertAttributes!)
            {
                yield return new ConverterMetadata(convertAttribute!.Command, serviceType, type, convertAttribute!.Lifetime);   
            }
        }

        private Type? GetConverterBase()
        {
            var current = type;
            
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ArgumentConverter<>))
                {
                    return current;
                }
                current = current.BaseType;
            }
            
            return null;
        }
    }
    
    internal static FrozenDictionary<string, Type> BuildConverterMap(ConverterMetadata[] metadata)
    {
        return metadata.ToDictionary(
                m => m.Command,
                m => m.ServiceType
            )
            .ToFrozenDictionary();
    }
}
