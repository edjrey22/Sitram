using MediatR;
using Sitram.Application.Pagos.Queries.ObtenerPago;

namespace Sitram.Application.Pagos.Queries.ObtenerPagoPorTramite;

/// <summary>Consulta el pago más reciente asociado a un trámite, si existe (para la UI del ciudadano).</summary>
public sealed record ObtenerPagoPorTramiteQuery(Guid TramiteId) : IRequest<PagoDto?>;
