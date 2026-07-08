using MediatR;
using Sitram.Domain.Seguridad;

namespace Sitram.Application.Seguridad.Queries.ListarIncidentesSeguridad;

public sealed class ListarIncidentesSeguridadQueryHandler(IIncidenteSeguridadRepository repositorio)
    : IRequestHandler<ListarIncidentesSeguridadQuery, IReadOnlyList<IncidenteSeguridadDto>>
{
    public async Task<IReadOnlyList<IncidenteSeguridadDto>> Handle(
        ListarIncidentesSeguridadQuery request, CancellationToken cancellationToken)
    {
        var incidentes = await repositorio.ListarAsync(cancellationToken);
        return incidentes.Select(MapearDto).ToList();
    }

    internal static IncidenteSeguridadDto MapearDto(IncidenteSeguridad i) => new(
        i.Id.Value, i.Titulo, i.Descripcion, i.Gravedad.ToString(), i.Estado.ToString(),
        i.FechaDeteccionUtc, i.FechaNotificacionUtc, i.OficialNotificadoId, i.Resolucion, i.FechaResolucionUtc);
}
