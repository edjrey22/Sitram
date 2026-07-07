using Microsoft.EntityFrameworkCore;
using Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación de <see cref="IAuditoriaReadService"/> (RF-071, solo lectura).</summary>
public sealed class AuditoriaReadService(SitramDbContext context) : IAuditoriaReadService
{
    public async Task<IReadOnlyList<EventoAuditoriaDto>> ListarPorTramiteAsync(
        Guid tramiteId, CancellationToken cancellationToken = default) =>
        await context.EventosAuditoria
            .AsNoTracking()
            .Where(e => e.TramiteId == tramiteId)
            .OrderBy(e => e.FechaUtc)
            .Select(e => new EventoAuditoriaDto(e.EventoId, e.Accion, e.DatosAntes, e.DatosDespues, e.FechaUtc))
            .ToListAsync(cancellationToken);
}
