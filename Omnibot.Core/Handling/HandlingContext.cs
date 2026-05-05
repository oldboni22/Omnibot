#region

using Microsoft.Extensions.ObjectPool;
using Omnibot.Core.Connector;

#endregion

namespace Omnibot.Core.Handling;

public sealed class HandlingContext : IResettable
{
    public IServiceProvider ServiceProvider { get; internal set; } = null!;

    public MessageContext MessageContext { get; internal set; } = null!;

    public CancellationToken CancellationToken { get; internal set; }
    
    public string PipeId { get; internal set; } = "";
    
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
    
    bool IResettable.TryReset()
    {
        ServiceProvider = null!;
        MessageContext = null!;
        CancellationToken = CancellationToken.None;
        PipeId = string.Empty;
        Items.Clear();
        return true;
    }
}
