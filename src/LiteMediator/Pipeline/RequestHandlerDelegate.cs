namespace LiteMediator;

/// <summary>
/// Representa o próximo elo da pipeline (o próximo behavior ou, no final, o handler real).
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);
