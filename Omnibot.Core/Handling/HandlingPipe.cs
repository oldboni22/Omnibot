namespace Omnibot.Core.Handling;

public delegate ValueTask HandlingDelegate(HandlingContext context);

public abstract class HandlingPipe()
{
    public abstract ValueTask Handle(HandlingContext context, HandlingDelegate next);
}
