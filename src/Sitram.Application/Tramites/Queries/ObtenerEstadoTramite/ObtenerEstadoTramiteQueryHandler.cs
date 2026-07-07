using MediatR;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Queries.ObtenerEstadoTramite;

/// <summary>Caso de uso de lectura: proyecta el agregado <see cref="Tramite"/> a un DTO.</summary>
public sealed class ObtenerEstadoTramiteQueryHandler(ITramiteRepository repositorio)
    : IRequestHandler<ObtenerEstadoTramiteQuery, TramiteEstadoDto?>
{
    public async Task<TramiteEstadoDto?> Handle(ObtenerEstadoTramiteQuery request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken);
        if (tramite is null)
            return null;

        var historial = tramite.Historial
            .OrderBy(a => a.FechaUtc)
            .Select(a => new ActuacionDto(
                a.EstadoAnterior.ToString(),
                a.EstadoNuevo.ToString(),
                a.Comentario,
                a.FechaUtc))
            .ToList();

        return new TramiteEstadoDto(
            tramite.Id.Value,
            tramite.Codigo,
            tramite.Estado.ToString(),
            tramite.CreadoUtc,
            historial);
    }
}
