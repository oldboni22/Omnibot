using System.Collections.Frozen;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.CommandHandling;
using Omnibot.MessageRouting.CommandHandling.PlatformRouting;
using Omnibot.MessageRouting.MessageParsing.CommandExtraction;

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
        
        public BotBuilder UseControllers(Assembly[]? assemblies = null, Action<PlatformRoutingBuilder>? configureRouting = null)
        {
            assemblies ??= [Assembly.GetCallingAssembly()];
            builder.Services.RegisterControllers(assemblies);
            
            FrozenSet<string>? routedCommands = null;
            FrozenDictionary<string, string>? connectorIdToPlatformKey = null;
            bool routeAll = false;

            if (configureRouting is not null)
            {
                var routingBuilder = new PlatformRoutingBuilder();
                configureRouting.Invoke(routingBuilder);

                (routedCommands, connectorIdToPlatformKey, routeAll) = routingBuilder;
            }
            
            return builder.Use<ControllerPipe>(sp =>
            {
                var handlers = sp.GetRequiredService<FrozenDictionary<string, Func<HandlingContext, Task>>>();
                return new ControllerPipe(handlers, routeAll, routedCommands, connectorIdToPlatformKey);
            });
        }
    }    
}
