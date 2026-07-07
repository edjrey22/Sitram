using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>El jefe de área rechaza el trámite: EnRevision → Rechazado (RF-028).</summary>
public sealed record RechazarTramiteCommand(Guid TramiteId, string Motivo) : IRequest;

public sealed class RechazarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<RechazarTramiteCommand>
{
    public async Task Handle(RechazarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.Rechazar(request.Motivo);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
