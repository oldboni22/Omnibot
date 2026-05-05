#region

using Microsoft.Extensions.DependencyInjection.Extensions;
using Omnibot.Core;

#endregion

namespace Omnibot.Auth;

public static class BotBuilderExtensions
{
    extension(BotBuilder botBuilder)
    {
        /// <summary>
        /// Adds authentication filters to the di.
        /// </summary>
        /// <param name="configure">
        /// The method used to configure authentication.
        /// </param>
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
