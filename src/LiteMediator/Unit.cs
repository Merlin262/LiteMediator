namespace LiteMediator;

/// <summary>
/// Representa um valor de retorno "vazio" para requests que não produzem resultado,
/// permitindo reutilizar a mesma pipeline genérica de <see cref="IRequestHandler{TRequest, TResponse}"/>.
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>A única instância de <see cref="Unit"/>.</summary>
    public static readonly Unit Value = default;

    /// <summary>Um <see cref="Task{Unit}"/> já concluído com <see cref="Value"/>, para evitar alocações repetidas.</summary>
    public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

    /// <inheritdoc/>
    public bool Equals(Unit other) => true;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc/>
    public override int GetHashCode() => 0;

    /// <inheritdoc/>
    public override string ToString() => "()";

    /// <summary>Sempre <see langword="true"/> — todas as instâncias de <see cref="Unit"/> são iguais.</summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>Sempre <see langword="false"/> — todas as instâncias de <see cref="Unit"/> são iguais.</summary>
    public static bool operator !=(Unit left, Unit right) => false;
}
