using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.IniciarSesion;

public sealed class IniciarSesionCommandHandler(
    IIdentityService identityService, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService,
    IEmailService emailService)
    : IRequestHandler<IniciarSesionCommand, IniciarSesionResultado>
{
    public async Task<IniciarSesionResultado> Handle(IniciarSesionCommand request, CancellationToken cancellationToken)
    {
        var resultado = await identityService.ValidarCredencialesAsync(request.UserName, request.Password, cancellationToken);

        if (resultado.BloqueadoTemporalmente)
            throw new AutenticacionInvalidaException("La cuenta está bloqueada temporalmente por intentos fallidos.");
        if (!resultado.Succeeded)
            throw new AutenticacionInvalidaException("Usuario o contraseña incorrectos.");

        if (await identityService.RequiereMfaAsync(resultado.UsuarioId, cancellationToken))
        {
            var codigo = await identityService.GenerarCodigoMfaAsync(resultado.UsuarioId, cancellationToken);
            var usuario = await identityService.ObtenerUsuarioAsync(resultado.UsuarioId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(usuario?.Email))
            {
                await emailService.EnviarAsync(
                    usuario.Email, "Tu código de verificación de SITRAM",
                    $"Tu código de verificación es: {codigo}. Ingrésalo para completar el inicio de sesión.",
                    cancellationToken);
            }

            return new IniciarSesionResultado(true, resultado.UsuarioId, null, null, null);
        }

        var permisos = await identityService.ObtenerPermisosAsync(resultado.UsuarioId, cancellationToken);
        var (accessToken, expiraUtc) = jwtTokenService.GenerarAccessToken(resultado.UsuarioId, request.UserName, permisos);
        var refreshToken = await refreshTokenService.EmitirAsync(resultado.UsuarioId, cancellationToken);

        return new IniciarSesionResultado(false, null, accessToken, expiraUtc, refreshToken);
    }
}
