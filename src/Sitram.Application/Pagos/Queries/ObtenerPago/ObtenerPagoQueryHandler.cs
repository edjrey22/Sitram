using MediatR;
using Sitram.Domain.Pagos;

namespace Sitram.Application.Pagos.Queries.ObtenerPago;

public sealed class ObtenerPagoQueryHandler(IPagoRepository repositorio) : IRequestHandler<ObtenerPagoQuery, PagoDto?>
{
    public async Task<PagoDto?> Handle(ObtenerPagoQuery request, CancellationToken cancellationToken)
    {
        var pago = await repositorio.ObtenerPorIdAsync(new PagoId(request.PagoId), cancellationToken);
        if (pago is null) return null;

        return new PagoDto(pago.Id.Value, pago.TramiteId.Value, pago.Monto, pago.Estado.ToString(), pago.ReferenciaPasarela, pago.FechaUtc);
    }
}
