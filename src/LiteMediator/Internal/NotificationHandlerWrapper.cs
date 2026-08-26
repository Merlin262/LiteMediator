using Microsoft.Extensions.DependencyInjection;

namespace LiteMediator.Internal;

internal abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(
        INotification notification,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

/// <summary>
/// Executa todos os <see cref="INotificationHandler{TNotification}"/> registrados, sequencialmente.
/// Se um handler lançar, os handlers seguintes não são chamados — comportamento "fail fast".
/// </summary>
internal sealed class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    public override async Task Handle(
        INotification notification,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typedNotification = (TNotification)notification;

        foreach (var handler in serviceProvider.GetServices<INotificationHandler<TNotification>>())
        {
            await handler.Handle(typedNotification, cancellationToken).ConfigureAwait(false);
        }
    }
}
