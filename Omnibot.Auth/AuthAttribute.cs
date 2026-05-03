#region

using Microsoft.Extensions.DependencyInjection;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.CommandHandling;

#endregion

namespace Omnibot.Auth;

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
