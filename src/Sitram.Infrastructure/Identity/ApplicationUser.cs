using Microsoft.AspNetCore.Identity;

namespace Sitram.Infrastructure.Identity;

/// <summary>
/// Usuario de Identity (USUARIO en modelo-datos.md). Los campos de contraseña, bloqueo
/// (<c>AccessFailedCount</c>/<c>LockoutEnd</c>) y MFA (<c>TwoFactorEnabled</c>) ya los provee
/// <see cref="IdentityUser{TKey}"/>; solo se añade la fecha de creación.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
}
