using Omnibot.Core.Connector;

namespace Omnibot.Core.Handling;

public sealed record HandlingContext(
    IServiceProvider ServiceProvider,
    MessageContext MessageContext,
    CancellationToken CancellationToken = default
)
{
    public string PipeId { get; internal set; } = "";
    
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
}
