using MediatR;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Auditoria.Queries.ConsultarAuditoria;

/// <summary>
/// Vista transversal del registro de auditoría, con filtros por usuario, acción y rango de
/// fechas (RF-071): a diferencia de <c>ObtenerAuditoriaTramiteQuery</c>, no se limita a un
/// trámite.
/// </summary>
public sealed record ConsultarAuditoriaQuery(
    Guid? UsuarioId, string? Accion, DateTime? Desde, DateTime? Hasta, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<EventoAuditoriaDetalleDto>>;

public sealed record EventoAuditoriaDetalleDto(
    long EventoId, Guid? TramiteId, Guid? UsuarioId, string Accion, string? DireccionIp, DateTime FechaUtc);
