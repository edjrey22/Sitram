using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>El ciudadano subsana la observación: Observado → EnRevision (RF-027).</summary>
public sealed record SubsanarTramiteCommand(Guid TramiteId) : IRequest;

public sealed class SubsanarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<SubsanarTramiteCommand>
{
    public async Task Handle(SubsanarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.Subsanar();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
