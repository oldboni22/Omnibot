namespace Omnibot.Core.Connector;

public interface IConnector
{
    Task StartAsync(CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<MessageContext> GetMessagesAsync(CancellationToken cancellationToken = default);
    
    string ConnectorIdentifier { get; }
    
    Task<bool> ShouldRetryToReconnect(Exception exception, CancellationToken cancellationToken = default);
}

