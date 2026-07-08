using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Seguridad.Commands.DesignarOficialDatos;

public sealed class DesignarOficialDatosCommandHandler(IIdentityService identityService, IAuditoriaService auditoriaService)
    : IRequestHandler<DesignarOficialDatosCommand>
{
    public async Task Handle(DesignarOficialDatosCommand request, CancellationToken cancellationToken)
    {
        var usuario = await identityService.ObtenerUsuarioAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException($"No existe el usuario {request.UsuarioId}.");

        await identityService.DesignarOficialDatosAsync(request.UsuarioId, cancellationToken);

        await auditoriaService.RegistrarAsync(
            null, "OficialDatosDesignado", null, usuario.UserName, cancellationToken);
    }
}
