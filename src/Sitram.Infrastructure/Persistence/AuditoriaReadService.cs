using Microsoft.EntityFrameworkCore;
using Sitram.Application.Auditoria.Queries.ConsultarAuditoria;
using Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;

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

    public async Task<PagedResult<EventoAuditoriaDetalleDto>> ConsultarAsync(
        Guid? usuarioId, string? accion, DateTime? desde, DateTime? hasta, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var consulta = context.EventosAuditoria.AsNoTracking().AsQueryable();

        if (usuarioId is not null) consulta = consulta.Where(e => e.UsuarioId == usuarioId);
        if (!string.IsNullOrWhiteSpace(accion)) consulta = consulta.Where(e => e.Accion == accion);
        if (desde is not null) consulta = consulta.Where(e => e.FechaUtc >= desde);
        if (hasta is not null) consulta = consulta.Where(e => e.FechaUtc <= hasta);

        var total = await consulta.CountAsync(cancellationToken);

        var items = await consulta
            .OrderByDescending(e => e.FechaUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EventoAuditoriaDetalleDto(e.EventoId, e.TramiteId, e.UsuarioId, e.Accion, e.DireccionIp, e.FechaUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<EventoAuditoriaDetalleDto>(items, total, page, pageSize);
    }
}
