namespace Omnibot.Core.Handling;

public sealed class InlinePipe(Func<HandlingContext, HandlingDelegate, ValueTask> lambda) : HandlingPipe
{
    public override ValueTask Handle(HandlingContext context, HandlingDelegate next)
    {
        return lambda(context, next);
    }
}
