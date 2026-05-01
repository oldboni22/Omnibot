using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.CommandHandling;
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

        public BotBuilder UseControllers()
        {
            builder.Services.RegisterControllers([Assembly.GetCallingAssembly()]);
            
            return builder.Use<ControllerPipe>();
        }
        
        public BotBuilder UseControllers(Assembly[] assemblies)
        {
            builder.Services.RegisterControllers(assemblies);
            
            return builder.Use<ControllerPipe>();
        }
    }    
}
