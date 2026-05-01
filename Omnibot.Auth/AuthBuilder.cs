namespace Omnibot.Auth;

public sealed class AuthBuilder
{
    internal readonly List<(string, Type)> HandlerTypes = [];

    internal AuthBuilder() {}
    
    public AuthBuilder AddPolicy<THandler>(string name) where THandler : AuthPolicyHandler
    {
        HandlerTypes.Add((name, typeof(THandler)));
        
        return this;
    }
}
