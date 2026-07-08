using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Application.Ciudadanos.Commands.RectificarDatos;

public sealed class RectificarDatosCiudadanoCommandHandler(ICiudadanoRepository repositorio, IUnitOfWork unitOfWork)
    : IRequestHandler<RectificarDatosCiudadanoCommand>
{
    public async Task Handle(RectificarDatosCiudadanoCommand request, CancellationToken cancellationToken)
    {
        var ciudadano = await repositorio.ObtenerPorIdAsync(new CiudadanoId(request.CiudadanoId), cancellationToken)
            ?? throw new NotFoundException($"No existe el ciudadano {request.CiudadanoId}.");

        ciudadano.Rectificar(request.Nombres, request.Apellidos, request.Correo, request.Telefono, request.Direccion);
        await unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
