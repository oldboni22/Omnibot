#region

using Microsoft.Extensions.Options;
using Omnibot.Core.Handling;

#endregion

namespace Omnibot.MessageRouting.MessageParsing.CommandExtraction;

internal sealed class ExtractCommandPipe(IOptions<CommandExtractionOptions> options) : HandlingPipe
{
    private readonly CommandExtractionOptions _options = options.Value;
    
    public override async Task Handle(HandlingContext context, HandlingDelegate next)
    {
        if (context.MessageContext.Message.StartsWith(_options.StartSymbol))
        {
            var(command, arguments) = ExtractCommand(context.MessageContext.Message);
            
            if(!string.IsNullOrEmpty(command))
            {
                context.MessageContext.Command = command;
                context.MessageContext.RawArgs = arguments;
            }
        }
        
        await next(context);
    }

    private static (string command, string? arguments) ExtractCommand(string message)
    {
        var span = message.AsSpan().Slice(1);
        
        var spaceIndex = span.IndexOf(' ');

        if (spaceIndex == -1)
        {
            return (span.ToString(), null);
        }
        
        var command = span.Slice(0, spaceIndex).ToString();
        var rawArgs = span.Slice(spaceIndex).TrimStart().ToString();
        
        return (command, rawArgs);
    }
}
