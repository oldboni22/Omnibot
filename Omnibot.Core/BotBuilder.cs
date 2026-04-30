using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Omnibot.Core.Connector;
using Omnibot.Core.Exceptions;
using Omnibot.Core.Handling;

namespace Omnibot.Core;

public sealed class BotBuilder
{
    private enum RegistrationType
    {
        Pipe,
        Connector
    }

    private bool _isLocked = false;
    
    private readonly HashSet<Type> _registeredTypes = new();
    
    private readonly List<IConnector> _connectors = [];
    
    private readonly List<Func<IServiceProvider, HandlingPipe>> _delegateFactories = [];

    private readonly HostApplicationBuilder _builder;
    
    public IServiceCollection Services => _builder.Services;
    
    public IConfiguration Configuration => _builder.Configuration;

    private Action<Exception> _operationalExceptionHandler = Console.WriteLine;

    private ushort _maxWorkers = 5; 
    
    private ushort _maxChannelSize = 1000; 
    
    private Func<HandlingContext, Dictionary<string, object>> _loggerScopeFactory = _ => [];
    
    private Func<HandlingContext, string> _pipeIdFactory = _ => Guid.NewGuid().ToString();

    internal BotBuilder()
    {
        _builder = Host.CreateApplicationBuilder();
    }

    public static BotBuilder Create() => new();
    
    private Bot Create(IServiceProvider serviceProvider)
    {
        return new Bot(
            _connectors.ToArray(),
            serviceProvider,
            BuildPipeline(serviceProvider),
            _maxWorkers,
            _maxChannelSize,
            _operationalExceptionHandler,
            _loggerScopeFactory,
            _pipeIdFactory
        );
    }

    public IHost Build()
    {
        SecureUnlocked();
        
        _isLocked = true;
        _builder.Services.AddHostedService(Create);
        
        return _builder.Build();
    }
    
    public BotBuilder AddHandlingContextAccessor()
    {
        SecureUnlocked();
        SecureUnique(typeof(HandlingContextPipe), RegistrationType.Pipe);
        
        Services.TryAddSingleton<IHandlingContextAccessor, HandlingContextAccessor>();
        _delegateFactories.Insert(0, sp => ActivatorUtilities.CreateInstance<HandlingContextPipe>(sp));
        
        return this;
    }
    
    public BotBuilder WithMaxWorkers(ushort maxWorkers)
    {
        SecureUnlocked();
        
        _maxWorkers = maxWorkers;
        return this;
    }

    public BotBuilder WithMaxChannelSize(ushort maxChannelSize)
    {
        SecureUnlocked();
        
        _maxChannelSize = maxChannelSize;
        return this;
    }
    
    public BotBuilder WithOperationalExceptionHandler(Action<Exception> handler)
    {
        SecureUnlocked();
        
        _operationalExceptionHandler = handler;
        return this;
    }

    public BotBuilder WithPipeIdFactory(Func<HandlingContext, string> pipeIdFactory)
    {
        SecureUnlocked();
        
        _pipeIdFactory = pipeIdFactory;
        return this;
    }

    public BotBuilder WithLoggerScopeFactory(Func<HandlingContext, Dictionary<string, object>> factory)
    {
        SecureUnlocked();
        
        _loggerScopeFactory = factory;
        return this;
    }
    
    public BotBuilder AddConnector(IConnector connector)
    {
        SecureUnlocked();
        SecureUnique(connector.GetType(), RegistrationType.Connector);
        
        _connectors.Add(connector);
        return this;
    }
    
    public BotBuilder AddConnector(Func<IConnector> factory)
    {
        SecureUnlocked();
        
        var connector = factory();
        SecureUnique(connector.GetType(), RegistrationType.Connector);
        _connectors.Add(connector);
        
        return this;
    }
    
    #region Pipeline

    public BotBuilder Use(Func<HandlingContext, HandlingDelegate, Task> lambda)
    {
        SecureUnlocked();
        
        _delegateFactories.Add(_ => new InlinePipe(lambda));
        return this;
    }
    
    public BotBuilder Use<TPipe>() where TPipe : HandlingPipe
    {
        SecureUnlocked();
        SecureUnique(typeof(TPipe), RegistrationType.Pipe);
        
        _delegateFactories.Add(sp => ActivatorUtilities.CreateInstance<TPipe>(sp));
        return this;
    }
    
    private HandlingDelegate BuildPipeline(IServiceProvider serviceProvider)
    {
        HandlingDelegate pipeline = _ => Task.CompletedTask;
        
        var instances = _delegateFactories
            .Select(factory => factory(serviceProvider))
            .ToArray();
        
        foreach (var pipe in instances.Reverse())
        {
            var currentNext = pipeline;
            pipeline = context => pipe.Handle(context, currentNext);
        }

        return pipeline;
    }
    
    #endregion
    
    private void SecureUnique(Type type, RegistrationType registrationType)
    {
        if (_registeredTypes.Add(type))
        {
            return;
        }

        Exception ex = registrationType switch
        {
            RegistrationType.Connector => new DuplicateConnectorException(type),
            _ => new DuplicatePipeException(type)
        };
        
        throw ex;
    }

    private void SecureUnlocked()
    {
        if (_isLocked)
        {
            throw new BotBuilderLockedException();
        }
    }
}
