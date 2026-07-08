using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Tramites.Queries.ObtenerReporteTramites;

public sealed class ObtenerReporteTramitesQueryHandler(ITramitesReadService readService)
    : IRequestHandler<ObtenerReporteTramitesQuery, ReporteTramitesDto>
{
    public Task<ReporteTramitesDto> Handle(ObtenerReporteTramitesQuery request, CancellationToken cancellationToken) =>
        readService.ObtenerReporteAsync(request.Desde, request.Hasta, cancellationToken);
}
