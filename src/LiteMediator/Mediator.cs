using System.Collections.Concurrent;
using LiteMediator.Internal;

namespace LiteMediator;

/// <summary>
/// Implementação padrão de <see cref="ISender"/> e <see cref="IPublisher"/>.
/// Resolve handlers via <see cref="IServiceProvider"/> e cacheia, por tipo concreto de
/// request/notification, o wrapper que encapsula a chamada genérica — assim a reflection
/// (via <see cref="Activator.CreateInstance(Type)"/>) acontece só uma vez por tipo, não a cada chamada.
/// </summary>
public sealed class Mediator : ISender, IPublisher
{
    private static readonly ConcurrentDictionary<Type, object> RequestHandlerWrappers = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationHandlerWrappers = new();

    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        var wrapper = (RequestHandlerWrapper<TResponse>)RequestHandlerWrappers.GetOrAdd(requestType, static rt =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(rt, typeof(TResponse));
            return Activator.CreateInstance(wrapperType)
                ?? throw new InvalidOperationException($"Não foi possível criar o wrapper para '{rt}'.");
        });

        return wrapper.Handle(request, _serviceProvider, cancellationToken);
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default) =>
        Send<Unit>(request, cancellationToken);

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        return PublishCore(notification, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (notification is not INotification typedNotification)
        {
            throw new ArgumentException(
                $"'{notification.GetType()}' precisa implementar {nameof(INotification)}.",
                nameof(notification));
        }

        return PublishCore(typedNotification, cancellationToken);
    }

    private Task PublishCore(INotification notification, CancellationToken cancellationToken)
    {
        var notificationType = notification.GetType();

        var wrapper = NotificationHandlerWrappers.GetOrAdd(notificationType, static nt =>
        {
            var wrapperType = typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(nt);
            return (NotificationHandlerWrapper)(Activator.CreateInstance(wrapperType)
                ?? throw new InvalidOperationException($"Não foi possível criar o wrapper para '{nt}'."));
        });

        return wrapper.Handle(notification, _serviceProvider, cancellationToken);
    }
}
