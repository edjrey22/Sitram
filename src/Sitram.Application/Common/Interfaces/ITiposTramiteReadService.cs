using Sitram.Application.TiposTramite.Queries.ObtenerCatalogoTramites;
using Sitram.Application.TiposTramite.Queries.ObtenerTipoTramiteDetalle;

namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto de lectura del catálogo de tipos de trámite (RF-014).</summary>
public interface ITiposTramiteReadService
{
    Task<IReadOnlyList<TipoTramiteResumenDto>> ListarActivosAsync(CancellationToken cancellationToken = default);

    Task<TipoTramiteDetalleDto?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken = default);
}
