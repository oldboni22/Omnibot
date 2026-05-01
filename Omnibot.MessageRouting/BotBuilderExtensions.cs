using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core;
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
    }    
}
