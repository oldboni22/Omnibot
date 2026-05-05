#region

using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.CommandHandling;

#endregion

namespace Omnibot.Auth;

/// <summary>
/// Enables authentication for a controller's method.
/// Can be used on a controller to enable authentication for all of its methods.
/// </summary>
/// <inheritdoc/>
/// <param name="policyNames"> The array of names of the policies that should be invoked in the authentication process.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthAttribute(int order, string[] policyNames) : ControllerFilterAttribute(order)
{
    public override async Task InvokeAsync(HandlingContext context, Func<HandlingContext, Task> next)
    {
        foreach (var policyName in policyNames)
        {
            var handler = context.ServiceProvider.GetRequiredKeyedService<AuthPolicyHandler>(policyName);
            handler.SetContext(context);
            
            if (!await handler.HandleAsync())
            {
                return;
            }
        }
        
        await next(context);
    }
}
