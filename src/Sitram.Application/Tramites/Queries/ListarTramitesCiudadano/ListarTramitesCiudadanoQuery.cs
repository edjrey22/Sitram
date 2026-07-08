using MediatR;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

/// <summary>Lista paginada de los trámites de un ciudadano (RF-050: bandeja del ciudadano).</summary>
public sealed record ListarTramitesCiudadanoQuery(Guid CiudadanoId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<TramiteResumenDto>>;

/// <summary>Vista resumida de un trámite para el listado (sin historial completo).</summary>
public sealed record TramiteResumenDto(Guid Id, string Codigo, string Estado, string TipoTramiteNombre, DateTime CreadoUtc);
