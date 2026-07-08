using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Auditoria.Queries.ConsultarAuditoria;

public sealed class ConsultarAuditoriaQueryHandler(IAuditoriaReadService readService)
    : IRequestHandler<ConsultarAuditoriaQuery, PagedResult<EventoAuditoriaDetalleDto>>
{
    public Task<PagedResult<EventoAuditoriaDetalleDto>> Handle(ConsultarAuditoriaQuery request, CancellationToken cancellationToken) =>
        readService.ConsultarAsync(
            request.UsuarioId, request.Accion, request.Desde, request.Hasta, request.Page, request.PageSize, cancellationToken);
}
