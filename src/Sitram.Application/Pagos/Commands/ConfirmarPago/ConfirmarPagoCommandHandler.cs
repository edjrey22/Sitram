using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Pagos;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Pagos.Commands.ConfirmarPago;

public sealed class ConfirmarPagoCommandHandler(
    IPagoRepository pagoRepositorio, ITramiteRepository tramiteRepositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmarPagoCommand>
{
    public async Task Handle(ConfirmarPagoCommand request, CancellationToken cancellationToken)
    {
        var pago = await pagoRepositorio.ObtenerPorIdAsync(new PagoId(request.PagoId), cancellationToken)
            ?? throw new NotFoundException($"No existe el pago {request.PagoId}.");

        pago.Confirmar();

        // RNF-032: ambos cambios se aplican en memoria antes de UN solo GuardarCambiosAsync,
        // por lo que EF Core los confirma en la misma transacción (todo o nada).
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(pago.TramiteId, cancellationToken);
        tramite?.IniciarRevision();

        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
