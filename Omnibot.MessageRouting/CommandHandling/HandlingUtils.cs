using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Omnibot.Core.Handling;
using Omnibot.MessageRouting.CommandHandling.PlatformRouting;
using Omnibot.MessageRouting.Exceptions;

namespace Omnibot.MessageRouting.CommandHandling;

public static class HandlingUtils
{
    extension(Assembly[] assemblies)
    {
        private Type[] GetValidControllerTypes()
        {
            return assemblies
                .AsParallel()
                .SelectMany(asm => asm.GetExportedTypes())
                .Where(t => t is { IsAbstract: false, IsInterface: false, IsGenericType: false })
                .Where(t => t.IsAssignableTo(typeof(Controller)))
                .ToArray();
        }
    }

    extension(IServiceCollection services)
    {
        internal IServiceCollection RegisterControllers(
            Assembly[] assemblies)
        {
            var controllerTypes = assemblies.GetValidControllerTypes();

            foreach (var type in controllerTypes)
            {
                services.TryAddScoped(type);
            }
            
            services.AddSingleton(BuildHandlers(controllerTypes));
            
            return services;
        }   
    }
    
    private static FrozenDictionary<string, Func<HandlingContext, Task>> BuildHandlers(
        Type[] controllerTypes)
    {
        var dict = new ConcurrentDictionary<string, Func<HandlingContext, Task>>();
        
        Parallel.ForEach(controllerTypes, type => HandleController(type, dict));
        
        return dict.ToFrozenDictionary();
    }

    private static void HandleController(
        Type controllerType, ConcurrentDictionary<string, Func<HandlingContext, Task>> dict)
    {
        var methods = controllerType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => Attribute.IsDefined(m, typeof(CommandAttribute)))
            .ToArray();

        var controllerFilters = controllerType.GetCustomAttributes<ControllerFilterAttribute>(true).ToArray();
        
        var platformAttribute = Attribute.GetCustomAttribute(controllerType, typeof(PlatformAttribute)) as  PlatformAttribute;
        var platformName = platformAttribute?.Platform;
        
        foreach (var method in methods)
        {
            HandleMethod(controllerType, controllerFilters, method, dict, platformName);
        }
    }

    private static void HandleMethod(
        Type controllerType,
        ControllerFilterAttribute[] controllerFilters, 
        MethodInfo method, 
        ConcurrentDictionary<string, Func<HandlingContext, Task>> dict,
        string? platformName)
    {
        var methodFilters = method.GetCustomAttributes<ControllerFilterAttribute>(true).ToArray();
        var allFilters = controllerFilters.Concat(methodFilters).ToArray();
        
        var commandAttr = method.GetCustomAttribute<CommandAttribute>()!;
        var command = commandAttr.Command;

        if (platformName is not null)
        {
            command = $"{platformName}.{command}";
        }
        
        if(!dict.TryAdd(command, BuildChain(controllerType, method, allFilters)))
        {
            throw new DuplicateCommandHandlingException(command);
        }
    }
    
    private static Func<HandlingContext, Task> BuildChain(
        Type controllerType, 
        MethodInfo method, 
        ControllerFilterAttribute[] allFilters)
    {
        var compiledAction = CompileMethod(controllerType, method);
        
        Func<HandlingContext, Task> pipeline = async context =>
        {
            var controller = context.ServiceProvider.GetRequiredService(controllerType);
            
            await compiledAction(controller, context);
        };
        
        foreach (var filter in allFilters.OrderByDescending(f => f.Order))
        {
            var nextStep = pipeline; 
            var currentFilter = filter;
            
            pipeline = context => currentFilter.InvokeAsync(context, nextStep);
        }

        return pipeline;
    }
    
    private static Func<object, HandlingContext, Task> CompileMethod(Type controllerType, MethodInfo method)
    {
        if (method.GetParameters().Length > 0 || method.ReturnParameter.ParameterType != typeof(Task))
        {
            throw new InvalidHandlingMethodSignatureException(method);
        }
        
        var controllerInstanceParam = Expression.Parameter(typeof(object), "controllerInstance");
        var contextParam = Expression.Parameter(typeof(HandlingContext), "context");

        var controllerCast = Expression.Convert(controllerInstanceParam, typeof(Controller));

        var setContextMethod = typeof(Controller).GetMethod(nameof(Controller.SetContext), BindingFlags.Instance | BindingFlags.NonPublic)!;
        var invokeSetContext = Expression.Call(controllerCast, setContextMethod, [contextParam]);
        
        var secondCast = Expression.Convert(invokeSetContext, controllerType);
        
        var call = Expression.Call(secondCast, method);
        
        return Expression.Lambda<Func<object, HandlingContext, Task>>(call, controllerInstanceParam, contextParam).Compile();
    }
}
