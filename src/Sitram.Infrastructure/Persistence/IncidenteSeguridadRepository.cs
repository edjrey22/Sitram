using Microsoft.EntityFrameworkCore;
using Sitram.Domain.Seguridad;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación del puerto <see cref="IIncidenteSeguridadRepository"/> sobre EF Core.</summary>
public sealed class IncidenteSeguridadRepository(SitramDbContext context) : IIncidenteSeguridadRepository
{
    public async Task AddAsync(IncidenteSeguridad incidente, CancellationToken cancellationToken = default) =>
        await context.IncidentesSeguridad.AddAsync(incidente, cancellationToken);

    public async Task<IncidenteSeguridad?> ObtenerPorIdAsync(IncidenteSeguridadId id, CancellationToken cancellationToken = default) =>
        await context.IncidentesSeguridad.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<IncidenteSeguridad>> ListarAsync(CancellationToken cancellationToken = default) =>
        await context.IncidentesSeguridad
            .OrderByDescending(i => i.FechaDeteccionUtc)
            .ToListAsync(cancellationToken);
}
