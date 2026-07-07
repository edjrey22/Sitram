using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites.Events;

/// <summary>El trámite fue observado; el ciudadano debe subsanar (RF-026/RF-051).</summary>
public sealed record TramiteObservadoEvent(TramiteId TramiteId, string Motivo) : IDomainEvent;
