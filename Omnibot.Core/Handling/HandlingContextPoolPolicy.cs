using Microsoft.Extensions.ObjectPool;

namespace Omnibot.Core.Handling;

internal sealed class HandlingContextPoolPolicy : IPooledObjectPolicy<HandlingContext>
{
    public HandlingContext Create() => new HandlingContext();

    public bool Return(HandlingContext obj)
    {
        return ((IResettable)obj).TryReset();
    }
}
