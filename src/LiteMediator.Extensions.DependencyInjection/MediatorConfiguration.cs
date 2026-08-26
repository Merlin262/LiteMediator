using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LiteMediator.Extensions.DependencyInjection;

/// <summary>
/// Opções usadas por <see cref="ServiceCollectionExtensions.AddLiteMediator"/> para saber quais
/// assemblies escanear em busca de handlers e quais pipeline behaviors registrar.
/// </summary>
public sealed class MediatorConfiguration
{
    internal HashSet<Assembly> AssembliesToScan { get; } = new();

    internal List<Type> OpenBehaviorsToRegister { get; } = new();

    /// <summary>
    /// Lifetime usado ao registrar <see cref="IRequestHandler{TRequest, TResponse}"/> e
    /// <see cref="INotificationHandler{TNotification}"/> encontrados via scanning.
    /// </summary>
    public ServiceLifetime HandlerLifetime { get; set; } = ServiceLifetime.Transient;

    public MediatorConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToScan.Add(assembly);
        return this;
    }

    public MediatorConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            AssembliesToScan.Add(assembly);
        }

        return this;
    }

    public MediatorConfiguration RegisterServicesFromAssemblyContaining<TMarker>() =>
        RegisterServicesFromAssembly(typeof(TMarker).Assembly);

    /// <summary>
    /// Registra um pipeline behavior genérico aberto (ex.: <c>typeof(LoggingBehavior&lt;,&gt;)</c>),
    /// aplicado a qualquer combinação de TRequest/TResponse. A ordem de chamada define a ordem na pipeline.
    /// </summary>
    public MediatorConfiguration AddOpenBehavior(Type openBehaviorType)
    {
        if (!openBehaviorType.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                "O tipo precisa ser uma definição genérica aberta, ex.: typeof(LoggingBehavior<,>).",
                nameof(openBehaviorType));
        }

        var implementsPipelineBehavior = openBehaviorType.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        if (!implementsPipelineBehavior)
        {
            throw new ArgumentException(
                $"'{openBehaviorType}' precisa implementar IPipelineBehavior<,>.",
                nameof(openBehaviorType));
        }

        OpenBehaviorsToRegister.Add(openBehaviorType);
        return this;
    }
}
