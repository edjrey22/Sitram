using MediatR;
using Sitram.Application.Seguridad.Queries.ListarIncidentesSeguridad;
using Sitram.Domain.Seguridad;

namespace Sitram.Application.Seguridad.Queries.ObtenerIncidenteSeguridad;

public sealed class ObtenerIncidenteSeguridadQueryHandler(IIncidenteSeguridadRepository repositorio)
    : IRequestHandler<ObtenerIncidenteSeguridadQuery, IncidenteSeguridadDto?>
{
    public async Task<IncidenteSeguridadDto?> Handle(ObtenerIncidenteSeguridadQuery request, CancellationToken cancellationToken)
    {
        var incidente = await repositorio.ObtenerPorIdAsync(new IncidenteSeguridadId(request.IncidenteId), cancellationToken);
        return incidente is null ? null : ListarIncidentesSeguridadQueryHandler.MapearDto(incidente);
    }
}
