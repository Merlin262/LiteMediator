namespace LiteMediator.Benchmarks;

// ---- baseline: nenhum mediator, chamada direta ao "handler" ----

public sealed class DirectPingHandler
{
    public Task<string> Handle(string message, CancellationToken cancellationToken) =>
        Task.FromResult($"Pong: {message}");
}

public sealed class DirectAuditHandler
{
    public Task Handle(string name, CancellationToken cancellationToken) => Task.CompletedTask;
}

// ---- LiteMediator ----

public sealed record LiteMediatorPing(string Message) : LiteMediator.IRequest<string>;

public sealed class LiteMediatorPingHandler : LiteMediator.IRequestHandler<LiteMediatorPing, string>
{
    public Task<string> Handle(LiteMediatorPing request, CancellationToken cancellationToken) =>
        Task.FromResult($"Pong: {request.Message}");
}

public sealed record LiteMediatorThingCreated(string Name) : LiteMediator.INotification;

public sealed class LiteMediatorAuditHandler : LiteMediator.INotificationHandler<LiteMediatorThingCreated>
{
    public Task Handle(LiteMediatorThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public sealed class LiteMediatorEmailHandler : LiteMediator.INotificationHandler<LiteMediatorThingCreated>
{
    public Task Handle(LiteMediatorThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public sealed class LiteMediatorSmsHandler : LiteMediator.INotificationHandler<LiteMediatorThingCreated>
{
    public Task Handle(LiteMediatorThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

// ---- MediatR ----

public sealed record MediatRPing(string Message) : MediatR.IRequest<string>;

public sealed class MediatRPingHandler : MediatR.IRequestHandler<MediatRPing, string>
{
    public Task<string> Handle(MediatRPing request, CancellationToken cancellationToken) =>
        Task.FromResult($"Pong: {request.Message}");
}

public sealed record MediatRThingCreated(string Name) : MediatR.INotification;

public sealed class MediatRAuditHandler : MediatR.INotificationHandler<MediatRThingCreated>
{
    public Task Handle(MediatRThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public sealed class MediatREmailHandler : MediatR.INotificationHandler<MediatRThingCreated>
{
    public Task Handle(MediatRThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}

public sealed class MediatRSmsHandler : MediatR.INotificationHandler<MediatRThingCreated>
{
    public Task Handle(MediatRThingCreated notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
