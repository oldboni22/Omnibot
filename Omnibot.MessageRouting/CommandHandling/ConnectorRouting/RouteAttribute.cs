namespace Omnibot.MessageRouting.CommandHandling.ConnectorRouting;

/// <summary>
/// Is used to indicate the connector route that the controller should handle. 
/// </summary>
/// <param name="routeKey">
/// The route key.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited =  false, AllowMultiple = true)]
public sealed class RouteAttribute(string routeKey) : Attribute
{
    public string RouteKey => routeKey;
}
