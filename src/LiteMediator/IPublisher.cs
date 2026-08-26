namespace LiteMediator;

/// <summary>
/// Publica uma notificação para todos os <see cref="INotificationHandler{TNotification}"/> registrados.
/// </summary>
public interface IPublisher
{
    /// <summary>Publica a notificação para todos os handlers registrados para <typeparamref name="TNotification"/>.</summary>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    /// <summary>
    /// Overload para cenários em que o tipo concreto da notificação só é conhecido em runtime.
    /// </summary>
    Task Publish(object notification, CancellationToken cancellationToken = default);
}
