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
        /// <summary>
        /// Adds command extraction to the pipeline.
        /// </summary>
        /// <param name="symbol">
        /// The start symbol of the command.
        /// If the symbol is not provided, tries to read the configuration.
        /// If the configuration is empty, uses '/' as the default value.
        /// </param>
        public BotBuilder UseCommandExtraction(char? symbol = null)
        {
            if(symbol is null)
            {
                builder.Services
                    .Configure<CommandExtractionOptions>(builder.Configuration.GetSection(CommandExtractionOptions.Section));
            }
            else
            {
                builder.Services.Configure<CommandExtractionOptions>(options =>
                {
                    options.StartSymbol = symbol.Value;
                });
            }
            
            return builder.Use<ExtractCommandPipe>();
        }

        /// <summary>
        /// Registers converter classes in the di and adds conversion to the pipeline.
        /// Requires command extraction.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies to scan for converter classes.
        /// By default, scans the calling assembly.
        /// </param>
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
        
        /// <summary>
        /// Registers controller classes in the di and adds controller routing and invocation to the pipeline.
        /// Requires command extraction.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies to scan for controller classes.
        /// By default, scans the calling assembly.
        /// </param>
        /// <param name="configureRouting">
        /// The method used for routing configuration.
        /// </param>
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
