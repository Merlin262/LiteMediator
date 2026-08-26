namespace LiteMediator;

/// <summary>
/// Envia um request para o handler correspondente, passando pela pipeline de
/// <see cref="IPipelineBehavior{TRequest, TResponse}"/> configurada.
/// </summary>
public interface ISender
{
    /// <summary>Envia o request e aguarda a resposta produzida pelo handler correspondente.</summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>Envia um request sem retorno.</summary>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
