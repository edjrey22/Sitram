using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Pagos.Commands.RegistrarPago;

public sealed class RegistrarPagoCommandHandler(
    ITramiteRepository tramiteRepositorio, ITipoTramiteRepository tipoTramiteRepositorio,
    IPagoRepository pagoRepositorio, IPagoService pagoService, IUnitOfWork unitOfWork)
    : IRequestHandler<RegistrarPagoCommand, RegistrarPagoResultado>
{
    public async Task<RegistrarPagoResultado> Handle(RegistrarPagoCommand request, CancellationToken cancellationToken)
    {
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        var tipoTramite = await tipoTramiteRepositorio.ObtenerPorIdAsync(tramite.TipoTramiteId, cancellationToken)
            ?? throw new TipoTramiteNoDisponibleException(tramite.TipoTramiteId);

        // RF-040: la tasa se calcula a partir del tipo de trámite.
        var pago = Pago.Registrar(tramite.Id, tipoTramite.Tasa);

        // Integración con la pasarela (modo prueba, RF-W01 fuera de alcance productivo).
        var referencia = await pagoService.RegistrarPagoAsync(pago.Monto, pago.Id.ToString(), cancellationToken);
        pago.AsignarReferenciaPasarela(referencia);

        await pagoRepositorio.AddAsync(pago, cancellationToken);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new RegistrarPagoResultado(pago.Id.Value, pago.Monto, referencia);
    }
}
