using MediatR;
using Sitram.Application.Seguridad.Queries.ListarIncidentesSeguridad;

namespace Sitram.Application.Seguridad.Queries.ObtenerIncidenteSeguridad;

/// <summary>Consulta el detalle de un incidente de seguridad.</summary>
public sealed record ObtenerIncidenteSeguridadQuery(Guid IncidenteId) : IRequest<IncidenteSeguridadDto?>;
