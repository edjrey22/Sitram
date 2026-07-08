using MediatR;
using Sitram.Application.Pagos.Queries.ObtenerPago;
using Sitram.Domain.Pagos;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Pagos.Queries.ObtenerPagoPorTramite;

public sealed class ObtenerPagoPorTramiteQueryHandler(IPagoRepository repositorio)
    : IRequestHandler<ObtenerPagoPorTramiteQuery, PagoDto?>
{
    public async Task<PagoDto?> Handle(ObtenerPagoPorTramiteQuery request, CancellationToken cancellationToken)
    {
        var pago = await repositorio.ObtenerPorTramiteAsync(new TramiteId(request.TramiteId), cancellationToken);
        if (pago is null) return null;

        return new PagoDto(pago.Id.Value, pago.TramiteId.Value, pago.Monto, pago.Estado.ToString(), pago.ReferenciaPasarela, pago.FechaUtc);
    }
}
