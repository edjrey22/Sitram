using Microsoft.EntityFrameworkCore;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Infrastructure.Identity;

/// <summary>
/// Semilla idempotente de roles y permisos (modelo-datos.md §6: "datos semilla... con HasData
/// o un seeder idempotente"). Se ejecuta al arrancar la Api.
/// </summary>
public static class IdentitySeeder
{
    private static readonly string[] Roles =
        ["Ciudadano", "MesaDePartes", "Revisor", "JefeDeArea", "Administrador", "Auditor", "OficialDatosPersonales"];

    /// <summary>Permiso → roles que lo poseen (tabla de ADR-0005).</summary>
    private static readonly (string Codigo, string[] Roles)[] Permisos =
    [
        ("tramite:iniciar", ["Ciudadano"]),
        ("tramite:enviar", ["Ciudadano"]),
        ("tramite:subsanar", ["Ciudadano"]),
        ("tramite:pagar", ["Ciudadano"]),
        ("tramite:adjuntar", ["Ciudadano"]),
        ("tramite:consultar", ["Ciudadano", "MesaDePartes", "Revisor", "JefeDeArea", "Administrador", "Auditor"]),
        ("tramite:recepcionar", ["MesaDePartes"]),
        ("tramite:evaluar", ["Revisor"]),
        ("tramite:observar", ["Revisor"]),
        ("tramite:aprobar", ["JefeDeArea"]),
        ("tramite:rechazar", ["JefeDeArea"]),
        ("administracion:gestionar", ["Administrador"]),
        ("auditoria:leer", ["Auditor"]),
        ("reportes:leer", ["JefeDeArea", "Administrador"]),
        ("datos:arco", ["OficialDatosPersonales"]),
    ];

    public static async Task SeedAsync(SitramDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var nombre in Roles)
        {
            if (!await context.Roles.AnyAsync(r => r.Nombre == nombre, cancellationToken))
                context.Roles.Add(new Rol { Nombre = nombre });
        }
        await context.SaveChangesAsync(cancellationToken);

        foreach (var (codigo, _) in Permisos)
        {
            if (!await context.Permisos.AnyAsync(p => p.Codigo == codigo, cancellationToken))
                context.Permisos.Add(new Permiso { Codigo = codigo });
        }
        await context.SaveChangesAsync(cancellationToken);

        var rolesPorNombre = await context.Roles.ToDictionaryAsync(r => r.Nombre, cancellationToken);
        var permisosPorCodigo = await context.Permisos.ToDictionaryAsync(p => p.Codigo, cancellationToken);

        foreach (var (codigo, rolesAsignados) in Permisos)
        {
            var permisoId = permisosPorCodigo[codigo].PermisoId;
            foreach (var nombreRol in rolesAsignados)
            {
                var rolId = rolesPorNombre[nombreRol].RolId;
                if (!await context.RolPermisos.AnyAsync(rp => rp.RolId == rolId && rp.PermisoId == permisoId, cancellationToken))
                    context.RolPermisos.Add(new RolPermiso { RolId = rolId, PermisoId = permisoId });
            }
        }
        await context.SaveChangesAsync(cancellationToken);
    }
}
