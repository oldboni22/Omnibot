#region

using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Omnibot.Core.Connector;
using Omnibot.Core.Exceptions;
using Omnibot.Core.Handling;

#endregion

namespace Omnibot.Core;

public sealed class Bot : BackgroundService
{
    private readonly Channel<MessageContext> _channel;

    private readonly IConnector[] _connectors;
    
    private readonly Action<Exception> _operationalExceptionHandler;
    
    private readonly IServiceProvider _serviceProvider;
    
    private readonly HandlingDelegate _handler;
    
    private readonly ushort _maxWorkers; 
    
    private readonly Func<HandlingContext, string> _pipeIdFactory;
    
    private readonly ObjectPool<HandlingContext> _contextPool;
    
    internal Bot(
        IConnector[] connectors, 
        IServiceProvider serviceProvider, 
        HandlingDelegate handler,
        ushort maxWorkers,
        ushort maxChannelSize,
        Action<Exception> operationalExceptionHandler, 
        Func<HandlingContext, string> pipeIdFactory)
    {
        _connectors = connectors;
        _serviceProvider = serviceProvider;
        _handler = handler;
        _operationalExceptionHandler = operationalExceptionHandler;
        _pipeIdFactory = pipeIdFactory;
        _maxWorkers = maxWorkers;
        _channel = Channel.CreateBounded<MessageContext>(new BoundedChannelOptions(maxChannelSize));
        
        _contextPool = _serviceProvider.GetRequiredService<ObjectPool<HandlingContext>>();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return StartListeningAsync(stoppingToken);
    }
    
    private async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        var producers = _connectors.Select(connector => ConnectWithRecoveryAsync(connector, cancellationToken));

        var workers = Enumerable.Range(0, _maxWorkers)
            .Select(_ => Task.Run(async () =>
            {
                try
                {
                    await foreach (var context in _channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        await ProcessMessageAsync(context, cancellationToken);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _operationalExceptionHandler(e);
                }
                
            }, cancellationToken));

        await Task.WhenAll(producers.Concat(workers));
    }

    private async Task ConnectWithRecoveryAsync(IConnector connector, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await connector.StartAsync(cancellationToken);
                
                await foreach (var message in connector.GetMessagesAsync(cancellationToken))
                {
                    await _channel.Writer.WriteAsync(message, cancellationToken);
                }
            }
            catch (OperationCanceledException) {}
            catch (Exception e)
            {
                if (await connector.ShouldRetryToReconnect(e, cancellationToken))
                {
                    continue;
                }
                
                _operationalExceptionHandler(new DeadConnectorException(connector, e));
                break;
            }
        }
    }
    
    private async Task ProcessMessageAsync(MessageContext messageContext, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var context = _contextPool.Get();
        context.ServiceProvider = scope.ServiceProvider;
        context.MessageContext = messageContext;
        context.CancellationToken = cancellationToken;
        context.PipeId = _pipeIdFactory(context);

        try
        {
            await _handler(context);
        }
        finally
        {
            _contextPool.Return(context);
        }
    }
}
