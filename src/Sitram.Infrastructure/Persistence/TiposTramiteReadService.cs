using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.TiposTramite.Queries.ObtenerCatalogoTramites;
using Sitram.Application.TiposTramite.Queries.ObtenerTipoTramiteDetalle;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación de <see cref="ITiposTramiteReadService"/> (RF-014, solo lectura).</summary>
public sealed class TiposTramiteReadService(SitramDbContext context) : ITiposTramiteReadService
{
    public async Task<IReadOnlyList<TipoTramiteResumenDto>> ListarActivosAsync(CancellationToken cancellationToken = default) =>
        await context.TiposTramite
            .AsNoTracking()
            .Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .Select(t => new TipoTramiteResumenDto(t.Id, t.Nombre, t.Descripcion, t.AreaResponsable, t.Tasa))
            .ToListAsync(cancellationToken);

    public async Task<TipoTramiteDetalleDto?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken = default)
    {
        var tipoTramite = await context.TiposTramite
            .AsNoTracking()
            .Include(t => t.Requisitos)
            .Include(t => t.PasosFlujo)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (tipoTramite is null) return null;

        return new TipoTramiteDetalleDto(
            tipoTramite.Id, tipoTramite.Nombre, tipoTramite.Descripcion, tipoTramite.AreaResponsable,
            tipoTramite.Tasa, tipoTramite.Activo,
            tipoTramite.Requisitos.Select(r => new RequisitoDocumentoDto(r.Id, r.Nombre, r.Obligatorio)).ToList(),
            tipoTramite.PasosFlujo.Select(p => new PasoFlujoDto(p.Id, p.Orden, p.RolResponsableId)).ToList());
    }
}
