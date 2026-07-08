using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.TiposTramite.Queries.ObtenerCatalogoTramites;

public sealed class ObtenerCatalogoTramitesQueryHandler(ITiposTramiteReadService readService)
    : IRequestHandler<ObtenerCatalogoTramitesQuery, IReadOnlyList<TipoTramiteResumenDto>>
{
    public Task<IReadOnlyList<TipoTramiteResumenDto>> Handle(ObtenerCatalogoTramitesQuery request, CancellationToken cancellationToken) =>
        readService.ListarActivosAsync(cancellationToken);
}
