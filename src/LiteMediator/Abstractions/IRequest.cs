namespace LiteMediator;

/// <summary>
/// Marca uma mensagem que espera uma resposta do tipo <typeparamref name="TResponse"/> ao ser
/// enviada via <see cref="ISender.Send{TResponse}"/>.
/// </summary>
public interface IRequest<out TResponse>
{
}

/// <summary>
/// Marca uma mensagem que não produz resultado (equivalente a <see cref="IRequest{TResponse}"/>
/// com <see cref="Unit"/>).
/// </summary>
public interface IRequest : IRequest<Unit>
{
}
