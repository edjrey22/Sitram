using MediatR;
using Sitram.Application.Auth.Commands.IniciarSesion;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.RefrescarToken;

public sealed class RefrescarTokenCommandHandler(
    IRefreshTokenService refreshTokenService, IIdentityService identityService, IJwtTokenService jwtTokenService)
    : IRequestHandler<RefrescarTokenCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(RefrescarTokenCommand request, CancellationToken cancellationToken)
    {
        var usuarioId = await refreshTokenService.ValidarYRevocarAsync(request.RefreshToken, cancellationToken)
            ?? throw new AutenticacionInvalidaException("El refresh token no es válido o ya expiró.");

        var usuario = await identityService.ObtenerUsuarioAsync(usuarioId, cancellationToken)
            ?? throw new AutenticacionInvalidaException("El usuario ya no existe.");

        var permisos = await identityService.ObtenerPermisosAsync(usuarioId, cancellationToken);
        var (accessToken, expiraUtc) = jwtTokenService.GenerarAccessToken(usuarioId, usuario.UserName, permisos);
        var nuevoRefreshToken = await refreshTokenService.EmitirAsync(usuarioId, cancellationToken);

        return new TokenResponse(accessToken, expiraUtc, nuevoRefreshToken);
    }
}
