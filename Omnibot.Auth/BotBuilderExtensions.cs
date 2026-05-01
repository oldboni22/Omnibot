using Microsoft.Extensions.DependencyInjection.Extensions;
using Omnibot.Core;

namespace Omnibot.Auth;

public static class BotBuilderExtensions
{
    extension(BotBuilder botBuilder)
    {
        public BotBuilder AddAuth(Action<AuthBuilder> configure)
        {
            var builder =  new AuthBuilder();
            configure(builder);

            foreach (var (name, type) in builder.HandlerTypes)
            {
                botBuilder.Services.TryAddKeyedScoped(typeof(AuthPolicyHandler), name, type);
            }

            return botBuilder;
        }
    }    
}
