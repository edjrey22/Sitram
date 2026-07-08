using Sitram.Domain.Common;

namespace Sitram.Domain.Ciudadanos.Events;

/// <summary>El ciudadano rectificó sus datos personales (RF-061; auditoría RF-070).</summary>
public sealed record DatosCiudadanoRectificadosEvent(CiudadanoId CiudadanoId) : IDomainEvent;
