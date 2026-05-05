using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.Auth;

public sealed class AuthBuilder
{
    private readonly HashSet<string> _registeredPolicies = [];
    
    internal readonly List<(string, Type)> HandlerTypes = [];

    internal AuthBuilder() {}
    
    /// <summary>
    /// Registers a policy with the given name.
    /// </summary>
    /// <param name="name">
    /// The name of the policy.
    /// </param>
    /// <typeparam name="THandler">
    /// The handler type used for policy handling.
    /// </typeparam>
    /// <exception cref="DuplicateAuthPolicyException">
    /// Is thrown when a policy with the given name is already registered.
    /// </exception>
    public AuthBuilder AddPolicy<THandler>(string name) where THandler : AuthPolicyHandler
    {
        if (!_registeredPolicies.Add(name))
        {
            throw new DuplicateAuthPolicyException(name);
        }
        
        HandlerTypes.Add((name, typeof(THandler)));
        
        return this;
    }
}
