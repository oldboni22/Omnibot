using Omnibot.Core.Handling;

namespace Omnibot.MessageRouting.CommandHandling;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class ControllerFilterAttribute(int order) : Attribute
{
    public int Order => order;
    
    public abstract Task InvokeAsync(HandlingContext context, Func<HandlingContext, Task> next);
}
