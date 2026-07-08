using MediatR;

namespace Sitram.Application.Seguridad.Commands.RegistrarIncidenteSeguridad;

/// <summary>Registra la detección de un incidente de seguridad y lo notifica de inmediato al Oficial de Datos Personales (RF-065).</summary>
public sealed record RegistrarIncidenteSeguridadCommand(string Titulo, string Descripcion, string Gravedad)
    : IRequest<RegistrarIncidenteSeguridadResultado>;

public sealed record RegistrarIncidenteSeguridadResultado(Guid IncidenteId, string Estado, bool OficialNotificado);
