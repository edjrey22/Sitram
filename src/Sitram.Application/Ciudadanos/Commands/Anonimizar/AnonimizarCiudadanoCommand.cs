using MediatR;

namespace Sitram.Application.Ciudadanos.Commands.Anonimizar;

/// <summary>Derecho al olvido: anonimiza los datos personales del ciudadano (RF-062).</summary>
public sealed record AnonimizarCiudadanoCommand(Guid CiudadanoId) : IRequest;
