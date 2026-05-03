#region

using Omnibot.Core.Handling;

#endregion

namespace Omnibot.MessageRouting.CommandHandling;

public abstract class Controller
{
    internal Controller SetContext(HandlingContext context)
    {
        Context = context;
        return this;
    }
    
    protected HandlingContext Context { get; private set; } = null!;
}
