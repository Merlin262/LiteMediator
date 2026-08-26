namespace LiteMediator;

/// <summary>
/// Processa um <typeparamref name="TRequest"/> e produz um <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Classe base de conveniência para handlers de requests sem retorno (<see cref="IRequest"/>),
/// evitando que o implementador precise lidar manualmente com <see cref="Unit"/>.
/// </summary>
public abstract class RequestHandlerBase<TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest
{
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }

    protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}
