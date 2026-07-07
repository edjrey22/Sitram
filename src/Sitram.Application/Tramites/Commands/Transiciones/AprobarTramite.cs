using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>El jefe de área aprueba el trámite: EnRevision → Aprobado (RF-028).</summary>
public sealed record AprobarTramiteCommand(Guid TramiteId) : IRequest;

public sealed class AprobarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<AprobarTramiteCommand>
{
    public async Task Handle(AprobarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.Aprobar();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
