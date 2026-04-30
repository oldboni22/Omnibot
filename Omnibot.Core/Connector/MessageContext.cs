namespace Omnibot.Core.Connector;

public sealed record MessageContext(
    IPlatformClient Client,
    string Message,
    string SenderId,
    string ChatId,
    string ConnectorIdentifier
    );
    