using MediatR;

namespace Sitram.Application.TiposTramite.Queries.ObtenerTipoTramiteDetalle;

/// <summary>Detalle de un tipo de trámite, con sus requisitos y su flujo de aprobación (RF-011, RF-012).</summary>
public sealed record ObtenerTipoTramiteDetalleQuery(int TipoTramiteId) : IRequest<TipoTramiteDetalleDto?>;

public sealed record TipoTramiteDetalleDto(
    int Id, string Nombre, string Descripcion, string AreaResponsable, decimal Tasa, bool Activo,
    IReadOnlyList<RequisitoDocumentoDto> Requisitos, IReadOnlyList<PasoFlujoDto> PasosFlujo);

public sealed record RequisitoDocumentoDto(int Id, string Nombre, bool Obligatorio);

public sealed record PasoFlujoDto(int Id, int Orden, int RolResponsableId);
