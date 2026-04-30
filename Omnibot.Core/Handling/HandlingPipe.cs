namespace Omnibot.Core.Handling;

public delegate Task HandlingDelegate(HandlingContext context);

public abstract class HandlingPipe()
{
    public abstract Task Handle(HandlingContext context, HandlingDelegate next);
}
