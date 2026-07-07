using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites.Events;

/// <summary>El trámite fue rechazado (dispara notificación al ciudadano, RF-028/RF-051).</summary>
public sealed record TramiteRechazadoEvent(TramiteId TramiteId, string Motivo) : IDomainEvent;
