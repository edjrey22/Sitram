using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;
using Sitram.Application.Tramites.Queries.ObtenerReporteTramites;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Implementación del lado de lectura: consulta el <see cref="SitramDbContext"/> directamente
/// con <c>AsNoTracking</c> y proyección a DTO (sin cargar el agregado ni su historial).
/// </summary>
public sealed class TramitesReadService(SitramDbContext context) : ITramitesReadService
{
    public async Task<PagedResult<TramiteResumenDto>> ListarPorCiudadanoAsync(
        Guid ciudadanoId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Tramites
            .AsNoTracking()
            .Where(t => t.CiudadanoId == ciudadanoId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreadoUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Join(context.TiposTramite, t => t.TipoTramiteId, tt => tt.Id, (t, tt) =>
                new TramiteResumenDto(t.Id.Value, t.Codigo, t.Estado.ToString(), tt.Nombre, t.CreadoUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<TramiteResumenDto>(items, total, page, pageSize);
    }

    public async Task<ReporteTramitesDto> ObtenerReporteAsync(
        DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var query = context.Tramites.AsNoTracking();
        if (desde is not null) query = query.Where(t => t.CreadoUtc >= desde.Value);
        if (hasta is not null) query = query.Where(t => t.CreadoUtc <= hasta.Value);

        var total = await query.CountAsync(cancellationToken);

        var porEstado = await query
            .GroupBy(t => t.Estado)
            .Select(g => new ConteoPorEstadoDto(g.Key.ToString(), g.Count()))
            .ToListAsync(cancellationToken);

        var porTipo = await query
            .Join(context.TiposTramite, t => t.TipoTramiteId, tt => tt.Id, (t, tt) => new { t, tt })
            .GroupBy(x => new { x.tt.Id, x.tt.Nombre })
            .Select(g => new ConteoPorTipoDto(g.Key.Id, g.Key.Nombre, g.Count()))
            .ToListAsync(cancellationToken);

        return new ReporteTramitesDto(total, porEstado, porTipo);
    }

    public async Task<IReadOnlyList<Guid>> ListarProximosAVencerAsync(
        int diasAnticipacion, CancellationToken cancellationToken = default)
    {
        var limite = DateTime.UtcNow.AddDays(diasAnticipacion);

        return await context.Tramites
            .AsNoTracking()
            .Where(t => t.Estado == Domain.Tramites.EstadoTramite.Observado
                     && !t.AlertaVencimientoEnviada
                     && t.FechaLimiteSubsanacionUtc != null
                     && t.FechaLimiteSubsanacionUtc <= limite)
            .Select(t => t.Id.Value)
            .ToListAsync(cancellationToken);
    }
}
