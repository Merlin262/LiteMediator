# LiteMediator

Implementação basica do padrão mediator/CQRS para .NET — pensada como alternativa livre .
Cobre o essencial usado no dia a dia com MediatR:

- `IRequest<TResponse>` / `IRequest` + `IRequestHandler<TRequest, TResponse>`
- `INotification` + `INotificationHandler<TNotification>` (publish/subscribe com múltiplos handlers)
- `ISender.Send(...)` e `IPublisher.Publish(...)`
- `IPipelineBehavior<TRequest, TResponse>` (middlewares/decorators em torno do handler)

O dispatch é feito via reflection cacheada por tipo de request/notification (mesma técnica usada historicamente pelo MediatR) — a reflection só acontece na primeira chamada de cada tipo; as seguintes usam apenas o cache.

## Instalação

```bash
dotnet add package LiteMediator.Core
dotnet add package LiteMediator.Extensions.DependencyInjection
```

## Uso

```csharp
// Program.cs
services.AddLiteMediator(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
```

```csharp
public sealed record Ping(string Message) : IRequest<string>;

public sealed class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken) =>
        Task.FromResult($"Pong: {request.Message}");
}

// em qualquer lugar com ISender injetado:
var response = await sender.Send(new Ping("oi"));
```

Request sem retorno:

```csharp
public sealed record DeleteThing(int Id) : IRequest;

public sealed class DeleteThingHandler : RequestHandlerBase<DeleteThing>
{
    protected override Task Handle(DeleteThing request, CancellationToken cancellationToken)
    {
        // ...
        return Task.CompletedTask;
    }
}
```

Notification com múltiplos handlers:

```csharp
public sealed record ThingCreated(string Name) : INotification;

public sealed class AuditLogHandler : INotificationHandler<ThingCreated> { /* ... */ }
public sealed class EmailHandler : INotificationHandler<ThingCreated> { /* ... */ }

await publisher.Publish(new ThingCreated("widget")); // chama os dois handlers
```

Pipeline behavior:

```csharp
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next(ct);
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        return response;
    }
}
```

## Migrando do MediatR

A API foi desenhada para ficar próxima da do MediatR — na maioria dos projetos, migrar é trocar o `using MediatR;` por `using LiteMediator;` e `services.AddMediatR(...)` por `services.AddLiteMediator(...)`. Principais diferenças:

| MediatR | LiteMediator |
|---|---|
| `IMediator` | `ISender` / `IPublisher` (separados) |
| `services.AddMediatR(cfg => ...)` | `services.AddLiteMediator(cfg => ...)` |
| `cfg.RegisterServicesFromAssembly(...)` | idêntico |
| `cfg.AddOpenBehavior(...)` | idêntico |

## Benchmarks

Projeto `benchmarks/LiteMediator.Benchmarks` compara LiteMediator, MediatR e chamada direta (BenchmarkDotNet). Rode com:

```bash
cd benchmarks/LiteMediator.Benchmarks
dotnet run -c Release
```

## Licença

[MIT](LICENSE.txt)
