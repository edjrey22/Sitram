using Sitram.Domain.Common;

namespace Sitram.Domain.Ciudadanos.Events;

/// <summary>El ciudadano ejerció el derecho al olvido (RF-062; auditoría RF-070).</summary>
public sealed record CiudadanoAnonimizadoEvent(CiudadanoId CiudadanoId) : IDomainEvent;
