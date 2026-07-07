using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sitram.Application.Common.Interfaces;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Infrastructure.Identity;

/// <summary>
/// Emite y rota refresh tokens (RNF-008). Solo se persiste el <b>hash</b> del token; el valor
/// en claro únicamente se entrega una vez al cliente.
/// </summary>
public sealed class RefreshTokenService(SitramDbContext context, IConfiguration configuration) : IRefreshTokenService
{
    public async Task<string> EmitirAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var dias = configuration.GetValue("Jwt:DiasExpiracionRefresh", 7);
        var tokenPlano = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            TokenHash = Hash(tokenPlano),
            CreadoUtc = DateTime.UtcNow,
            ExpiraUtc = DateTime.UtcNow.AddDays(dias),
        });
        await context.SaveChangesAsync(cancellationToken);

        return tokenPlano;
    }

    public async Task<Guid?> ValidarYRevocarAsync(string refreshTokenPlano, CancellationToken cancellationToken = default)
    {
        var hash = Hash(refreshTokenPlano);
        var registro = await context.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (registro is null || registro.RevocadoUtc is not null || registro.ExpiraUtc <= DateTime.UtcNow)
            return null;

        registro.RevocadoUtc = DateTime.UtcNow; // rotación: un refresh token se usa una sola vez
        await context.SaveChangesAsync(cancellationToken);

        return registro.UsuarioId;
    }

    private static string Hash(string valor) =>
        Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(valor)));
}
