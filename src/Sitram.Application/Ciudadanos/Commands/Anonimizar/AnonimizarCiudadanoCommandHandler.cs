using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Ciudadanos.Commands.Anonimizar;

public sealed class AnonimizarCiudadanoCommandHandler(ICiudadanoRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<AnonimizarCiudadanoCommand>
{
    public async Task Handle(AnonimizarCiudadanoCommand request, CancellationToken cancellationToken)
    {
        var ciudadano = await repositorio.ObtenerPorIdAsync(new CiudadanoId(request.CiudadanoId), cancellationToken)
            ?? throw new NotFoundException($"No existe el ciudadano {request.CiudadanoId}.");

        ciudadano.Anonimizar();
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
