using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Ciudadanos.Commands.OtorgarConsentimiento;

public sealed class OtorgarConsentimientoCommandHandler(ICiudadanoRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<OtorgarConsentimientoCommand>
{
    public async Task Handle(OtorgarConsentimientoCommand request, CancellationToken cancellationToken)
    {
        var ciudadano = await repositorio.ObtenerPorIdAsync(new CiudadanoId(request.CiudadanoId), cancellationToken)
            ?? throw new NotFoundException($"No existe el ciudadano {request.CiudadanoId}.");

        ciudadano.OtorgarConsentimiento(request.Finalidad);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
