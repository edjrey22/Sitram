namespace Sitram.Domain.Ciudadanos;

/// <summary>Puerto de persistencia del agregado <see cref="Ciudadano"/> (implementado en Infrastructure).</summary>
public interface ICiudadanoRepository
{
    Task AddAsync(Ciudadano ciudadano, CancellationToken cancellationToken = default);

    Task<Ciudadano?> ObtenerPorIdAsync(CiudadanoId id, CancellationToken cancellationToken = default);
}
