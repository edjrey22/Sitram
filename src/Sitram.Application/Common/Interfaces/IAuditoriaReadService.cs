using Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;

namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto de lectura del registro de auditoría (RF-071).</summary>
public interface IAuditoriaReadService
{
    Task<IReadOnlyList<EventoAuditoriaDto>> ListarPorTramiteAsync(
        Guid tramiteId, CancellationToken cancellationToken = default);
}
