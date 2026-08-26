namespace LiteMediator;

/// <summary>
/// Representa um valor de retorno "vazio" para requests que não produzem resultado,
/// permitindo reutilizar a mesma pipeline genérica de <see cref="IRequestHandler{TRequest, TResponse}"/>.
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = default;

    public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

    public bool Equals(Unit other) => true;

    public override bool Equals(object? obj) => obj is Unit;

    public override int GetHashCode() => 0;

    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;
}
