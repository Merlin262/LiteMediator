namespace LiteMediator;

/// <summary>
/// Reage a uma <typeparamref name="TNotification"/> publicada. Múltiplos handlers podem existir
/// para a mesma notificação — todos serão invocados por <see cref="IPublisher.Publish"/>.
/// </summary>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}
