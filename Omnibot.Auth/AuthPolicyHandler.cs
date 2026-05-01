using Omnibot.Core.Handling;

namespace Omnibot.Auth;

public abstract class AuthPolicyHandler
{
    internal void SetContext(HandlingContext context)
    {
        Context = context;
    }
    
    protected HandlingContext Context { get; private set; } = null!;

    public abstract Task<bool> HandleAsync();
}
