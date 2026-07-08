using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>Mesa de Partes admite el expediente: Recibido → EnRevision (RF-024).</summary>
public sealed record IniciarRevisionTramiteCommand(Guid TramiteId) : IRequest;

public sealed class IniciarRevisionTramiteCommandHandler(
    ITramiteRepository repositorio, ITipoTramiteRepository tipoTramiteRepositorio,
    IPagoRepository pagoRepositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<IniciarRevisionTramiteCommand>
{
    public async Task Handle(IniciarRevisionTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        // RF-043: impedir el avance de un trámite con tasa impaga (los tipos gratuitos, Tasa = 0, no requieren pago).
        var tipoTramite = await tipoTramiteRepositorio.ObtenerPorIdAsync(tramite.TipoTramiteId, cancellationToken);
        if (tipoTramite is not null && tipoTramite.Tasa > 0)
        {
            var pago = await pagoRepositorio.ObtenerPorTramiteAsync(tramite.Id, cancellationToken);
            if (pago is null || pago.Estado != EstadoPago.Confirmado)
                throw new PagoRequeridoException(request.TramiteId);
        }

        tramite.IniciarRevision();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
