namespace Sitram.Infrastructure.Identity;

/// <summary>Rol de negocio (ROL en modelo-datos.md): Ciudadano, MesaDePartes, Revisor, etc.</summary>
public sealed class Rol
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = default!;
}
