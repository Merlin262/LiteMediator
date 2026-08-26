namespace LiteMediator.Tests;

public sealed record Ping(string Message) : IRequest<string>;

public sealed class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken) =>
        Task.FromResult($"Pong: {request.Message}");
}

public sealed record DeleteThing(int Id) : IRequest;

public sealed class DeleteThingHandler : RequestHandlerBase<DeleteThing>
{
    public List<int> DeletedIds { get; } = new();

    protected override Task Handle(DeleteThing request, CancellationToken cancellationToken)
    {
        DeletedIds.Add(request.Id);
        return Task.CompletedTask;
    }
}

public sealed record ThingCreated(string Name) : INotification;

public sealed class AuditLogHandler : INotificationHandler<ThingCreated>
{
    public List<string> Logged { get; } = new();

    public Task Handle(ThingCreated notification, CancellationToken cancellationToken)
    {
        Logged.Add($"audit:{notification.Name}");
        return Task.CompletedTask;
    }
}

public sealed class EmailHandler : INotificationHandler<ThingCreated>
{
    public List<string> Sent { get; } = new();

    public Task Handle(ThingCreated notification, CancellationToken cancellationToken)
    {
        Sent.Add($"email:{notification.Name}");
        return Task.CompletedTask;
    }
}

public class RecordingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public static List<string> CallOrder { get; } = new();

    private readonly string _name;

    public RecordingBehavior(string name) => _name = name;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        CallOrder.Add($"{_name}:before");
        var response = await next(cancellationToken);
        CallOrder.Add($"{_name}:after");
        return response;
    }
}

public sealed class FirstBehavior<TRequest, TResponse> : RecordingBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public FirstBehavior() : base("first") { }
}

public sealed class SecondBehavior<TRequest, TResponse> : RecordingBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public SecondBehavior() : base("second") { }
}
