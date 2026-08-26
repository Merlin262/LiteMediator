using Microsoft.Extensions.DependencyInjection;

namespace LiteMediator.Internal;

/// <summary>
/// Base não-genérica que permite ao <see cref="Mediator"/> guardar um wrapper por tipo concreto de
/// request num dicionário único, mesmo sem conhecer TRequest em tempo de compilação.
/// </summary>
internal abstract class RequestHandlerWrapper<TResponse>
{
    public abstract Task<TResponse> Handle(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

/// <summary>
/// Instanciado via reflection (<c>Activator.CreateInstance</c>) uma única vez por tipo de request e
/// cacheado — a partir daí toda chamada usa apenas resolução de serviços do DI, sem reflection extra.
/// </summary>
internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override Task<TResponse> Handle(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Task<TResponse> Handler(CancellationToken ct)
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return handler.Handle((TRequest)request, ct);
        }

        var pipeline = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate(
                (RequestHandlerDelegate<TResponse>)Handler,
                (next, behavior) => ct => behavior.Handle((TRequest)request, next, ct));

        return pipeline(cancellationToken);
    }
}
