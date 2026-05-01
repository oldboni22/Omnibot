using System.Collections.Frozen;
using Omnibot.Core.Handling;

namespace Omnibot.MessageRouting.CommandHandling;

public sealed class ControllerPipe(FrozenDictionary<string, Func<HandlingContext, Task>> Handlers) : HandlingPipe
{
    public override async Task Handle(HandlingContext context, HandlingDelegate next)
    {
        var command = context.MessageContext.Command;

        if (command is null)
        {
            await next(context);
            return;
        }

        if (Handlers.TryGetValue(command, out var handler))
        {
            await handler(context);
        }
        
        await next(context);
    }
}
