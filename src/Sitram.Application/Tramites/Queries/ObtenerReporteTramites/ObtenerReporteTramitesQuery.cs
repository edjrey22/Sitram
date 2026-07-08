using MediatR;

namespace Sitram.Application.Tramites.Queries.ObtenerReporteTramites;

/// <summary>
/// Reporte de trámites por estado, tipo y periodo (RF-072); <c>Desde</c>/<c>Hasta</c> filtran
/// por <c>CreadoUtc</c> y son opcionales (si se omiten, cubre todo el histórico).
/// </summary>
public sealed record ObtenerReporteTramitesQuery(DateTime? Desde, DateTime? Hasta) : IRequest<ReporteTramitesDto>;

public sealed record ReporteTramitesDto(
    int TotalTramites, IReadOnlyList<ConteoPorEstadoDto> PorEstado, IReadOnlyList<ConteoPorTipoDto> PorTipo);

public sealed record ConteoPorEstadoDto(string Estado, int Cantidad);

public sealed record ConteoPorTipoDto(int TipoTramiteId, string NombreTipo, int Cantidad);
