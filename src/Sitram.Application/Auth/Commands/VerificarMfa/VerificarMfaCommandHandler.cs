using MediatR;
using Sitram.Application.Auth.Commands.IniciarSesion;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Auth.Commands.VerificarMfa;

public sealed class VerificarMfaCommandHandler(
    IIdentityService identityService, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
    : IRequestHandler<VerificarMfaCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(VerificarMfaCommand request, CancellationToken cancellationToken)
    {
        var valido = await identityService.VerificarCodigoMfaAsync(request.UsuarioId, request.Codigo, cancellationToken);
        if (!valido)
            throw new AutenticacionInvalidaException("Código de verificación inválido o expirado.");

        var usuario = await identityService.ObtenerUsuarioAsync(request.UsuarioId, cancellationToken)
            ?? throw new AutenticacionInvalidaException("Usuario no encontrado.");

        var permisos = await identityService.ObtenerPermisosAsync(request.UsuarioId, cancellationToken);
        var (accessToken, expiraUtc) = jwtTokenService.GenerarAccessToken(request.UsuarioId, usuario.UserName, permisos);
        var refreshToken = await refreshTokenService.EmitirAsync(request.UsuarioId, cancellationToken);

        return new TokenResponse(accessToken, expiraUtc, refreshToken);
    }
}
