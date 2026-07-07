namespace Sitram.Infrastructure.Identity;

/// <summary>Relación rol → permiso (ROL_PERMISO en modelo-datos.md).</summary>
public sealed class RolPermiso
{
    public int RolId { get; set; }
    public int PermisoId { get; set; }
}
