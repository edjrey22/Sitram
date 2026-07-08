using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.RestablecerContrasena;

public sealed class RestablecerContrasenaCommandHandler(IIdentityService identityService)
    : IRequestHandler<RestablecerContrasenaCommand>
{
    public async Task Handle(RestablecerContrasenaCommand request, CancellationToken cancellationToken)
    {
        var restablecido = await identityService.RestablecerContrasenaAsync(
            request.UsuarioId, request.Token, request.NuevaContrasena, cancellationToken);

        if (!restablecido)
            throw new AutenticacionInvalidaException("El token de recuperación es inválido, ya expiró o la contraseña no cumple la política.");
    }
}
