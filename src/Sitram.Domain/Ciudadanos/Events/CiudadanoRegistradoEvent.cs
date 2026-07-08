using Sitram.Domain.Common;

namespace Sitram.Domain.Ciudadanos.Events;

/// <summary>Se registró el perfil de un ciudadano (RF-070: auditoría).</summary>
public sealed record CiudadanoRegistradoEvent(CiudadanoId CiudadanoId) : IDomainEvent;
