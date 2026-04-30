using Omnibot.Core.Connector;

namespace Omnibot.Core.Exceptions;

public class DeadConnectorException(IConnector connector, Exception inner) : Exception($"The connector {connector.ConnectorIdentifier} was not recovered.", inner);
