using Sitram.Domain.Common;

namespace Sitram.Domain.Seguridad.Events;

/// <summary>
/// Se notificó un incidente de seguridad al Oficial de Datos Personales activo (RF-065,
/// D.S. 016-2024-JUS). <see cref="OficialNotificadoId"/> es nulo si no hay ningún oficial
/// designado (RF-066) al momento de la detección.
/// </summary>
public sealed record IncidenteSeguridadNotificadoEvent(
    IncidenteSeguridadId IncidenteId, Guid? OficialNotificadoId, string Titulo, GravedadIncidente Gravedad) : IDomainEvent;
