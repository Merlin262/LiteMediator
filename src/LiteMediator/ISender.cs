namespace LiteMediator;

/// <summary>
/// Envia um request para o handler correspondente, passando pela pipeline de
/// <see cref="IPipelineBehavior{TRequest, TResponse}"/> configurada.
/// </summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
