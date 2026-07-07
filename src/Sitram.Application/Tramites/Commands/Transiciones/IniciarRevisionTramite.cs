using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>Mesa de Partes admite el expediente: Recibido → EnRevision (RF-024).</summary>
public sealed record IniciarRevisionTramiteCommand(Guid TramiteId) : IRequest;

public sealed class IniciarRevisionTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<IniciarRevisionTramiteCommand>
{
    public async Task Handle(IniciarRevisionTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.IniciarRevision();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
