namespace Sitram.Domain.Tramites;

/// <summary>Puerto de persistencia del agregado <see cref="Tramite"/> (se implementa en Infrastructure).</summary>
public interface ITramiteRepository
{
    Task AddAsync(Tramite tramite, CancellationToken cancellationToken = default);

    Task<Tramite?> ObtenerPorIdAsync(TramiteId id, CancellationToken cancellationToken = default);
}
