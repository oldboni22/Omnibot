#region

using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core.Handling;

#endregion

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

        if (context.ServiceProvider.GetRequiredService(converterType) is not IArgumentConverter converter)
        {
            return next(context);
        }
        
        object? converted = null;
        
        try
        {
            converted = converter?.ConvertUntyped(context.MessageContext.RawArgs);
        }
        catch
        {
            if (converter!.PropagateException)
            {
                throw;
            }
        }

        context.MessageContext.ConvertedArgs = converted;
        
        return next(context);
    }
}
