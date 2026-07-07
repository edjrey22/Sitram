using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tramites.Commands.Transiciones;

/// <summary>El ciudadano envía el trámite: Borrador → Recibido (RF-023).</summary>
public sealed record EnviarTramiteCommand(Guid TramiteId) : IRequest;

public sealed class EnviarTramiteCommandHandler(ITramiteRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<EnviarTramiteCommand>
{
    public async Task Handle(EnviarTramiteCommand request, CancellationToken cancellationToken)
    {
        var tramite = await repositorio.ObtenerPorIdAsync(new TramiteId(request.TramiteId), cancellationToken)
            ?? throw new NotFoundException($"No existe el trámite {request.TramiteId}.");

        tramite.Enviar();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
