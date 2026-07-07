using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;

public sealed class ObtenerAuditoriaTramiteQueryHandler(IAuditoriaReadService readService)
    : IRequestHandler<ObtenerAuditoriaTramiteQuery, IReadOnlyList<EventoAuditoriaDto>>
{
    public Task<IReadOnlyList<EventoAuditoriaDto>> Handle(ObtenerAuditoriaTramiteQuery request, CancellationToken cancellationToken) =>
        readService.ListarPorTramiteAsync(request.TramiteId, cancellationToken);
}
