using MediatR;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

public sealed class ListarTramitesCiudadanoQueryHandler(ITramitesReadService readService)
    : IRequestHandler<ListarTramitesCiudadanoQuery, PagedResult<TramiteResumenDto>>
{
    public Task<PagedResult<TramiteResumenDto>> Handle(ListarTramitesCiudadanoQuery request, CancellationToken cancellationToken) =>
        readService.ListarPorCiudadanoAsync(request.CiudadanoId, request.Page, request.PageSize, cancellationToken);
}
