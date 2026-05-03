#region

using System.Collections.Frozen;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Omnibot.Core;
using Omnibot.MessageRouting.CommandHandling;
using Omnibot.MessageRouting.CommandHandling.ConnectorRouting;
using Omnibot.MessageRouting.MessageParsing.ArgumentConversion;
using Omnibot.MessageRouting.MessageParsing.CommandExtraction;

#endregion

namespace Omnibot.MessageRouting;

public static class BotBuilderExtensions
{
    extension(BotBuilder builder)
    {
        public BotBuilder UseCommandExtraction(char symbol)
        {
            builder.Services.Configure<CommandExtractionOptions>(options =>
            {
                options.StartSymbol = symbol;
            });
            
            return builder.Use<ExtractCommandPipe>();
        }

        public BotBuilder UseCommandExtraction()
        {
            builder.Services.Configure<CommandExtractionOptions>(builder.Configuration.GetSection(CommandExtractionOptions.Section));
            
            return builder.Use<ExtractCommandPipe>();
        }

        public BotBuilder UseArgumentConversion(Assembly[]? assemblies = null)
        {
            assemblies ??= [Assembly.GetCallingAssembly()];

            var metadata = assemblies.GetValidConvertersMetadata();

            foreach (var data in metadata)
            {
                var descriptor = new ServiceDescriptor(
                    data.ServiceType,
                    data.ImplementationType,
                    data.ServiceLifetime
                );
                
                builder.Services.TryAdd(descriptor);
            }

            return builder.Use(_ => new ArgumentConversionPipe(ConversionUtils.BuildConverterMap(metadata)));
        }
        
        public BotBuilder UseControllers(Assembly[]? assemblies = null, Action<ConnectorRoutingBuilder>? configureRouting = null)
        {
            assemblies ??= [Assembly.GetCallingAssembly()];
            
            var controllerTypes = assemblies.GetValidControllerTypes();

            foreach (var type in controllerTypes)
            {
                builder.Services.TryAddScoped(type);
            }
            
            FrozenSet<string>? routedCommands = null;
            FrozenDictionary<string, string>? connectorIdToPlatformKey = null;
            var routeAll = false;

            if (configureRouting is not null)
            {
                var routingBuilder = new ConnectorRoutingBuilder();
                configureRouting.Invoke(routingBuilder);

                (routedCommands, connectorIdToPlatformKey, routeAll) = routingBuilder;
            }
            
            return builder.Use<ControllerPipe>(_ => 
                new ControllerPipe(HandlingUtils.BuildHandlers(controllerTypes), routeAll, routedCommands, connectorIdToPlatformKey));
        }
    }    
}
