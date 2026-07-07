using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites.Events;

/// <summary>El trámite fue aprobado (dispara resolución y notificación, RF-028/RF-051).</summary>
public sealed record TramiteAprobadoEvent(TramiteId TramiteId) : IDomainEvent;
