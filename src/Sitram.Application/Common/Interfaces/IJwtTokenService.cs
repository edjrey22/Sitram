namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto para la emisión de tokens de acceso JWT (ADR-0005).</summary>
public interface IJwtTokenService
{
    /// <summary>Genera un JWT de vida corta (RNF-008) con los permisos como claims.</summary>
    (string Token, DateTime ExpiraUtc) GenerarAccessToken(Guid usuarioId, string userName, IEnumerable<string> permisos);
}
