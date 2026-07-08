namespace Sitram.Domain.Seguridad;

/// <summary>Puerto de persistencia del agregado <see cref="IncidenteSeguridad"/> (implementado en Infrastructure).</summary>
public interface IIncidenteSeguridadRepository
{
    Task AddAsync(IncidenteSeguridad incidente, CancellationToken cancellationToken = default);

    Task<IncidenteSeguridad?> ObtenerPorIdAsync(IncidenteSeguridadId id, CancellationToken cancellationToken = default);

    /// <summary>Todos los incidentes, más recientes primero (para el Oficial de Datos, RF-066).</summary>
    Task<IReadOnlyList<IncidenteSeguridad>> ListarAsync(CancellationToken cancellationToken = default);
}
