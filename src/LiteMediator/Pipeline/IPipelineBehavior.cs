namespace LiteMediator;

/// <summary>
/// Um elo de middleware em torno do handler de um request. Vários behaviors registrados para o
/// mesmo <typeparamref name="TRequest"/>/<typeparamref name="TResponse"/> são encadeados na ordem
/// em que foram registrados no container de DI.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Processa o request. Pode inspecionar/alterar o request, short-circuitar a chamada a
    /// <paramref name="next"/>, ou processar a resposta antes de retorná-la.
    /// </summary>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
