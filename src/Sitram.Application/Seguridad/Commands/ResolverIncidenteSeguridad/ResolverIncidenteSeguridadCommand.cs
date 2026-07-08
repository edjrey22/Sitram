using MediatR;

namespace Sitram.Application.Seguridad.Commands.ResolverIncidenteSeguridad;

/// <summary>Cierra un incidente de seguridad notificado, dejando constancia de la resolución aplicada (RF-065).</summary>
public sealed record ResolverIncidenteSeguridadCommand(Guid IncidenteId, string Resolucion) : IRequest;
