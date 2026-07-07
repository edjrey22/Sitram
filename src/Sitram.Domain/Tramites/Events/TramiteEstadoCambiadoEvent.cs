using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites.Events;

/// <summary>
/// Hook uniforme para <b>toda</b> transición de estado del trámite (RF-070: auditoría de cada
/// acción). Se emite una vez por cada llamada a <c>CambiarEstado</c>, independientemente del
/// evento específico (Aprobado/Rechazado/Observado) que también se emita.
/// </summary>
public sealed record TramiteEstadoCambiadoEvent(
    TramiteId TramiteId, EstadoTramite EstadoAnterior, EstadoTramite EstadoNuevo, string? Comentario) : IDomainEvent;
