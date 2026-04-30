namespace Omnibot.Core.Handling;

public sealed class InlinePipe(Func<HandlingContext, HandlingDelegate, Task> lambda) : HandlingPipe
{
    public override Task Handle(HandlingContext context, HandlingDelegate next)
    {
        return lambda(context, next);
    }
}
