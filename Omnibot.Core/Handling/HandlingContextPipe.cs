namespace Omnibot.Core.Handling;

internal sealed class HandlingContextPipe(IHandlingContextAccessor accessor) : HandlingPipe
{
    public override ValueTask Handle(HandlingContext context, HandlingDelegate next)
    {
        try
        {
            accessor.Context = context;
            return next(context);
        }
        finally
        {
            accessor.Context = null!;
        }
    }
}
