using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>El revisor observa el trámite: EnRevision → Observado (RF-026).</summary>
public sealed record ObservarTramiteCommand(Guid TramiteId, string Motivo) : IRequest;

public sealed class ObservarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<ObservarTramiteCommand>
{
    public async Task Handle(ObservarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.Observar(request.Motivo);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
