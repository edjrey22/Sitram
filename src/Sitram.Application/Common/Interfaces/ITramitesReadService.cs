using Sitram.Application.Common.Models;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

namespace Sitram.Application.Common.Interfaces;

/// <summary>
/// Puerto de lectura del lado de consultas (CQRS): proyecta directo a DTO sin pasar por el
/// repositorio de agregados, evitando el sobre-fetching en listados (errores-conocidos 2.1).
/// </summary>
public interface ITramitesReadService
{
    Task<PagedResult<TramiteResumenDto>> ListarPorCiudadanoAsync(
        Guid ciudadanoId, int page, int pageSize, CancellationToken cancellationToken = default);
}
