using MediatR;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Ciudadanos.Queries.ObtenerPerfilCiudadano;

public sealed class ObtenerPerfilCiudadanoQueryHandler(ICiudadanoRepository repositorio)
    : IRequestHandler<ObtenerPerfilCiudadanoQuery, CiudadanoPerfilDto?>
{
    public async Task<CiudadanoPerfilDto?> Handle(ObtenerPerfilCiudadanoQuery request, CancellationToken cancellationToken)
    {
        var ciudadano = await repositorio.ObtenerPorIdAsync(new CiudadanoId(request.CiudadanoId), cancellationToken);
        if (ciudadano is null) return null;

        var consentimientos = ciudadano.Consentimientos
            .Select(c => new ConsentimientoDto(c.Id, c.Finalidad, c.Otorgado, c.FechaUtc, c.RevocadoUtc))
            .ToList();

        return new CiudadanoPerfilDto(
            ciudadano.Id.Value, ciudadano.Nombres, ciudadano.Apellidos, ciudadano.Dni.Valor, ciudadano.Correo,
            ciudadano.Telefono, ciudadano.Direccion, ciudadano.EstaAnonimizado, ciudadano.CreadoUtc, consentimientos);
    }
}
