using MediatR;

namespace Sitram.Application.TiposTramite.Queries.ObtenerCatalogoTramites;

/// <summary>Catálogo de tipos de trámite activos, visible al ciudadano (RF-014).</summary>
public sealed record ObtenerCatalogoTramitesQuery : IRequest<IReadOnlyList<TipoTramiteResumenDto>>;

public sealed record TipoTramiteResumenDto(int Id, string Nombre, string Descripcion, string AreaResponsable, decimal Tasa);
