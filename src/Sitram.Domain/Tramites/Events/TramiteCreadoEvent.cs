using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites.Events;

/// <summary>Se creó un trámite nuevo en Borrador (RF-020; auditoría RF-070).</summary>
public sealed record TramiteCreadoEvent(TramiteId TramiteId) : IDomainEvent;
