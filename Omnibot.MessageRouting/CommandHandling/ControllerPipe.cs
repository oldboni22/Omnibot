using System.Collections.Frozen;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.MessageRouting.CommandHandling;

public sealed class ControllerPipe(
    FrozenDictionary<string, Func<HandlingContext, Task>> handlers,
    FrozenSet<string>? routedCommands,
    FrozenDictionary<string, string>? connectorIdToPlatformKey) : HandlingPipe
{
    public override async Task Handle(HandlingContext context, HandlingDelegate next)
    {
        var command = context.MessageContext.Command;

        if (command is null)
        {
            await next(context);
            return;
        }

        var routeCommand = routedCommands is not null && routedCommands.Contains(command);
        
        if (routeCommand)
        {
            if (!connectorIdToPlatformKey!.TryGetValue(context.MessageContext.ConnectorIdentifier, out var platformKey))
            {
                throw new ConnectorRoutingNotConfiguredException(context.MessageContext.ConnectorIdentifier);
            }
            command = $"{platformKey}.{command}";
        }
        
        if (handlers.TryGetValue(command, out var handler))
        {
            await handler(context);
        }
        
        await next(context);
    }
}
