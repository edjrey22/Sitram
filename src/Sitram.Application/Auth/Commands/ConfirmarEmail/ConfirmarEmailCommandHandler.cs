using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.ConfirmarEmail;

public sealed class ConfirmarEmailCommandHandler(IIdentityService identityService) : IRequestHandler<ConfirmarEmailCommand>
{
    public async Task Handle(ConfirmarEmailCommand request, CancellationToken cancellationToken)
    {
        var confirmado = await identityService.ConfirmarEmailAsync(request.UsuarioId, request.Token, cancellationToken);
        if (!confirmado)
            throw new AutenticacionInvalidaException("El token de confirmación es inválido o ya expiró.");
    }
}
