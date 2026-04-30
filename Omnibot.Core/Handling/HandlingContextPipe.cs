namespace Omnibot.Core.Handling;

internal sealed class HandlingContextPipe(IHandlingContextAccessor accessor) : HandlingPipe
{
    public override async Task Handle(HandlingContext context, HandlingDelegate next)
    {
        try
        {
            accessor.Context = context;
            await next(context);
        }
        finally
        {
            accessor.Context = null!;
        }
    }
}
