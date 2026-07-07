namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto para la emisión y rotación de refresh tokens (RNF-008).</summary>
public interface IRefreshTokenService
{
    Task<string> EmitirAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida el refresh token y lo revoca (rotación: nunca se reutiliza). Devuelve el
    /// identificador del usuario si era válido, o <c>null</c> si no existe, expiró o ya fue usado.
    /// </summary>
    Task<Guid?> ValidarYRevocarAsync(string refreshTokenPlano, CancellationToken cancellationToken = default);
}
