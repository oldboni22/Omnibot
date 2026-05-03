using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core.Handling;

namespace Omnibot.MessageRouting.MessageParsing.ArgumentConversion;

public sealed class ArgumentConversionPipe(FrozenDictionary<string, Type> converters) : HandlingPipe
{
    public override Task Handle(HandlingContext context, HandlingDelegate next)
    {
        if (context.MessageContext.Command is null)
        {
            return next(context);
        }
        
        if(!converters.TryGetValue(context.MessageContext.Command, out var converterType))
        {
            return next(context);
        }

        var converter = context.ServiceProvider.GetRequiredService(converterType) as IArgumentConverter;

        object? converted;
        
        try
        {
            converted = converter?.ConvertUntyped(context.MessageContext.RawArgs);
        }
        catch
        {
            converted = null;
        }

        context.MessageContext.ConvertedArgs = converted;
        
        return next(context);
    }
}
