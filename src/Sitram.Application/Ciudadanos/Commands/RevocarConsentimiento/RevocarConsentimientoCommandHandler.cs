using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Ciudadanos.Commands.RevocarConsentimiento;

public sealed class RevocarConsentimientoCommandHandler(ICiudadanoRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<RevocarConsentimientoCommand>
{
    public async Task Handle(RevocarConsentimientoCommand request, CancellationToken cancellationToken)
    {
        var ciudadano = await repositorio.ObtenerPorIdAsync(new CiudadanoId(request.CiudadanoId), cancellationToken)
            ?? throw new NotFoundException($"No existe el ciudadano {request.CiudadanoId}.");

        ciudadano.RevocarConsentimiento(request.Finalidad);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
