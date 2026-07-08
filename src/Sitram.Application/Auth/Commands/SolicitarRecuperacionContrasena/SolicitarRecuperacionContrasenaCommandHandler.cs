using MediatR;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.SolicitarRecuperacionContrasena;

public sealed class SolicitarRecuperacionContrasenaCommandHandler(
    IIdentityService identityService, IEmailService emailService)
    : IRequestHandler<SolicitarRecuperacionContrasenaCommand>
{
    public async Task Handle(SolicitarRecuperacionContrasenaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await identityService.ObtenerUsuarioPorEmailAsync(request.Email, cancellationToken);
        if (usuario is null)
            return; // no se revela si el correo existe (evita enumeración de usuarios)

        var token = await identityService.GenerarTokenRecuperacionContrasenaAsync(usuario.Id, cancellationToken);
        var enlace = $"https://sitram.local/restablecer-contrasena?usuarioId={usuario.Id}&token={Uri.EscapeDataString(token)}";

        await emailService.EnviarAsync(
            request.Email, "Recupera tu contraseña en SITRAM",
            $"Hola {usuario.UserName}, para restablecer tu contraseña ingresa a: {enlace}", cancellationToken);
    }
}
