namespace Sitram.Domain.TiposTramite;

/// <summary>Puerto de persistencia del agregado <see cref="TipoTramite"/> (implementado en Infrastructure).</summary>
public interface ITipoTramiteRepository
{
    Task AddAsync(TipoTramite tipoTramite, CancellationToken cancellationToken = default);

    Task<TipoTramite?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
