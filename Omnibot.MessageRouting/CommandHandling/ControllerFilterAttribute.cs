#region

using Omnibot.Core.Handling;

#endregion

namespace Omnibot.MessageRouting.CommandHandling;

/// <summary>
/// Encapsulates a piece of shared logic that should be executed before the controller method invocation.
/// </summary>
/// <param name="order">
/// Is used to determine the order of filter invocation.
/// The smaller the value, the higher the priority. 
/// </param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class ControllerFilterAttribute(int order) : Attribute
{
    public int Order => order;
    
    public abstract Task InvokeAsync(HandlingContext context, Func<HandlingContext, Task> next);
}
