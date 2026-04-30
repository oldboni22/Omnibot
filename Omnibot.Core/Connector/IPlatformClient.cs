namespace Omnibot.Core.Connector;

public interface IPlatformClient
{
    Task SendAsync(string message, string chatId, CancellationToken cancellationToken = default);
}
