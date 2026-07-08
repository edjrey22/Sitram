using Sitram.Application.Auditoria.Queries.ConsultarAuditoria;
using Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto de lectura del registro de auditoría (RF-071).</summary>
public interface IAuditoriaReadService
{
    Task<IReadOnlyList<EventoAuditoriaDto>> ListarPorTramiteAsync(
        Guid tramiteId, CancellationToken cancellationToken = default);

    /// <summary>Vista transversal (no limitada a un trámite), filtrable y paginada.</summary>
    Task<PagedResult<EventoAuditoriaDetalleDto>> ConsultarAsync(
        Guid? usuarioId, string? accion, DateTime? desde, DateTime? hasta, int page, int pageSize,
        CancellationToken cancellationToken = default);
}
