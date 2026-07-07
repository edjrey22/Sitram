namespace Sitram.Infrastructure.Identity;

/// <summary>Refresh token rotativo (RNF-008): se almacena solo su hash, nunca en claro.</summary>
public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string TokenHash { get; set; } = default!;
    public DateTime CreadoUtc { get; set; }
    public DateTime ExpiraUtc { get; set; }
    public DateTime? RevocadoUtc { get; set; }
}
