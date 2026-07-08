using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.TiposTramite.Queries.ObtenerTipoTramiteDetalle;

public sealed class ObtenerTipoTramiteDetalleQueryHandler(ITiposTramiteReadService readService)
    : IRequestHandler<ObtenerTipoTramiteDetalleQuery, TipoTramiteDetalleDto?>
{
    public Task<TipoTramiteDetalleDto?> Handle(ObtenerTipoTramiteDetalleQuery request, CancellationToken cancellationToken) =>
        readService.ObtenerDetalleAsync(request.TipoTramiteId, cancellationToken);
}
