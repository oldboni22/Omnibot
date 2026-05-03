#region

using Omnibot.Core.Handling;

#endregion

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
