namespace Sitram.Infrastructure.Identity;

/// <summary>Relación usuario → rol (USUARIO_ROL en modelo-datos.md).</summary>
public sealed class UsuarioRol
{
    public Guid UsuarioId { get; set; }
    public int RolId { get; set; }
}
