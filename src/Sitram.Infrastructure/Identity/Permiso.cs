namespace Sitram.Infrastructure.Identity;

/// <summary>Permiso atómico (PERMISO en modelo-datos.md), p. ej. "tramite:aprobar".</summary>
public sealed class Permiso
{
    public int PermisoId { get; set; }
    public string Codigo { get; set; } = default!;
}
