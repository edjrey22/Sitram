using Sitram.Application.Common.Models;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;
using Sitram.Application.Tramites.Queries.ObtenerReporteTramites;

namespace Sitram.Application.Common.Interfaces;

/// <summary>
/// Puerto de lectura del lado de consultas (CQRS): proyecta directo a DTO sin pasar por el
/// repositorio de agregados, evitando el sobre-fetching en listados (errores-conocidos 2.1).
/// </summary>
public interface ITramitesReadService
{
    Task<PagedResult<TramiteResumenDto>> ListarPorCiudadanoAsync(
        Guid ciudadanoId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Reporte agregado de trámites por estado y tipo, en un periodo opcional (RF-072).</summary>
    Task<ReporteTramitesDto> ObtenerReporteAsync(
        DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    /// <summary>Ids de trámites Observado cuyo plazo vence dentro de <paramref name="diasAnticipacion"/> días y aún no fueron alertados (RF-053).</summary>
    Task<IReadOnlyList<Guid>> ListarProximosAVencerAsync(int diasAnticipacion, CancellationToken cancellationToken = default);
}
