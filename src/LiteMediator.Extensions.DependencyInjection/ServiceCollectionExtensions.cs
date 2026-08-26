using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LiteMediator.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra <see cref="ISender"/>/<see cref="IPublisher"/> (implementados por <see cref="Mediator"/>)
    /// e escaneia os assemblies configurados em busca de <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// e <see cref="INotificationHandler{TNotification}"/> concretos.
    /// </summary>
    public static IServiceCollection AddLiteMediator(
        this IServiceCollection services,
        Action<MediatorConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new MediatorConfiguration();
        configure(configuration);

        services.TryAddTransient<Mediator>();
        services.TryAddTransient<ISender>(sp => sp.GetRequiredService<Mediator>());
        services.TryAddTransient<IPublisher>(sp => sp.GetRequiredService<Mediator>());

        foreach (var assembly in configuration.AssembliesToScan)
        {
            RegisterClosedGenericImplementations(services, assembly, typeof(IRequestHandler<,>), configuration.HandlerLifetime);
            RegisterClosedGenericImplementations(services, assembly, typeof(INotificationHandler<>), configuration.HandlerLifetime);
        }

        foreach (var openBehaviorType in configuration.OpenBehaviorsToRegister)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), openBehaviorType);
        }

        return services;
    }

    private static void RegisterClosedGenericImplementations(
        IServiceCollection services,
        Assembly assembly,
        Type openGenericInterface,
        ServiceLifetime lifetime)
    {
        foreach (var type in GetLoadableTypes(assembly))
        {
            if (type is not { IsAbstract: false, IsInterface: false })
            {
                continue;
            }

            foreach (var implementedInterface in type.GetInterfaces())
            {
                if (!implementedInterface.IsGenericType ||
                    implementedInterface.GetGenericTypeDefinition() != openGenericInterface)
                {
                    continue;
                }

                services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, type, lifetime));
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
