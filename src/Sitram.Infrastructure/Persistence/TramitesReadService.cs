using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Implementación del lado de lectura: consulta el <see cref="SitramDbContext"/> directamente
/// con <c>AsNoTracking</c> y proyección a DTO (sin cargar el agregado ni su historial).
/// </summary>
public sealed class TramitesReadService(SitramDbContext context) : ITramitesReadService
{
    public async Task<PagedResult<TramiteResumenDto>> ListarPorCiudadanoAsync(
        Guid ciudadanoId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Tramites
            .AsNoTracking()
            .Where(t => t.CiudadanoId == ciudadanoId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreadoUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TramiteResumenDto(t.Id.Value, t.Codigo, t.Estado.ToString(), t.CreadoUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<TramiteResumenDto>(items, total, page, pageSize);
    }
}
