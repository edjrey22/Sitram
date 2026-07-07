using MediatR;

namespace Sitram.Application.Auditoria.Queries.ObtenerAuditoriaTramite;

/// <summary>Consulta de solo lectura del registro de auditoría de un trámite (RF-071).</summary>
public sealed record ObtenerAuditoriaTramiteQuery(Guid TramiteId) : IRequest<IReadOnlyList<EventoAuditoriaDto>>;

public sealed record EventoAuditoriaDto(
    long EventoId, string Accion, string? DatosAntes, string? DatosDespues, DateTime FechaUtc);
