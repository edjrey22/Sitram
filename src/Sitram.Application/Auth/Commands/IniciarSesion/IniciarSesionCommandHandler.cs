using MediatR;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.IniciarSesion;

public sealed class IniciarSesionCommandHandler(
    IIdentityService identityService, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
    : IRequestHandler<IniciarSesionCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(IniciarSesionCommand request, CancellationToken cancellationToken)
    {
        var resultado = await identityService.ValidarCredencialesAsync(request.UserName, request.Password, cancellationToken);

        if (resultado.BloqueadoTemporalmente)
            throw new AutenticacionInvalidaException("La cuenta está bloqueada temporalmente por intentos fallidos.");
        if (!resultado.Succeeded)
            throw new AutenticacionInvalidaException("Usuario o contraseña incorrectos.");

        var permisos = await identityService.ObtenerPermisosAsync(resultado.UsuarioId, cancellationToken);
        var (accessToken, expiraUtc) = jwtTokenService.GenerarAccessToken(resultado.UsuarioId, request.UserName, permisos);
        var refreshToken = await refreshTokenService.EmitirAsync(resultado.UsuarioId, cancellationToken);

        return new TokenResponse(accessToken, expiraUtc, refreshToken);
    }
}
