namespace Sitram.Domain.Common;

/// <summary>Objeto del dominio con identidad propia y ciclo de vida.</summary>
public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id) => Id = id;

    // Requerido por EF Core
    protected Entity() { }

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other &&
        GetType() == other.GetType() &&
        EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode() => Id.GetHashCode();
}
