using MediatR;

namespace Sitram.Application.Ciudadanos.Queries.ObtenerPerfilCiudadano;

/// <summary>Perfil completo del ciudadano, en formato interoperable (RF-060, portabilidad).</summary>
public sealed record ObtenerPerfilCiudadanoQuery(Guid CiudadanoId) : IRequest<CiudadanoPerfilDto?>;

public sealed record CiudadanoPerfilDto(
    Guid Id, string Nombres, string Apellidos, string Dni, string Correo, string Telefono, string Direccion,
    bool EstaAnonimizado, DateTime CreadoUtc, IReadOnlyList<ConsentimientoDto> Consentimientos);

public sealed record ConsentimientoDto(Guid Id, string Finalidad, bool Otorgado, DateTime FechaUtc, DateTime? RevocadoUtc);
