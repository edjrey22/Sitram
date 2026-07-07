using FluentValidation;
using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.Registrar;

public sealed class RegistrarCiudadanoCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegistrarCiudadanoCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarCiudadanoCommand request, CancellationToken cancellationToken)
    {
        var resultado = await identityService.CrearUsuarioAsync(
            request.UserName, request.Email, request.Password, cancellationToken);

        if (!resultado.Succeeded)
            throw new ValidationException(
                resultado.Errores.Select(e => new FluentValidation.Results.ValidationFailure(nameof(request.Password), e)));

        // TODO(SITRAM): enviar correo de verificación con IEmailService (Sprint de notificaciones).
        return resultado.UsuarioId;
    }
}
