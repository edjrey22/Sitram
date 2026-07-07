namespace Sitram.Domain.Common;

/// <summary>
/// Marcador no genérico para recolectar los eventos de dominio de cualquier agregado
/// (<see cref="AggregateRoot{TId}"/> es genérico sobre <c>TId</c>, por lo que el despachador
/// de eventos —en Infrastructure— necesita un tipo común para consultarlos con EF Core).
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
