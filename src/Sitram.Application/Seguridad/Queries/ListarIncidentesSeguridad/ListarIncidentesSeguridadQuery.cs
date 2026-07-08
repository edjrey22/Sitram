using MediatR;

namespace Sitram.Application.Seguridad.Queries.ListarIncidentesSeguridad;

/// <summary>Lista los incidentes de seguridad registrados, para el Oficial de Datos Personales (RF-066).</summary>
public sealed record ListarIncidentesSeguridadQuery : IRequest<IReadOnlyList<IncidenteSeguridadDto>>;

public sealed record IncidenteSeguridadDto(
    Guid Id, string Titulo, string Descripcion, string Gravedad, string Estado,
    DateTime FechaDeteccionUtc, DateTime? FechaNotificacionUtc, Guid? OficialNotificadoId,
    string? Resolucion, DateTime? FechaResolucionUtc);
