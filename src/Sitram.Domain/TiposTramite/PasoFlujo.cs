using Sitram.Domain.Common;

namespace Sitram.Domain.TiposTramite;

/// <summary>
/// Paso del flujo de aprobación de un tipo de trámite (RF-012). <see cref="RolResponsableId"/>
/// referencia al rol de RBAC (tabla propia en Infrastructure) solo por su identificador: el
/// dominio no conoce la entidad Rol, evitando así una dependencia hacia Infrastructure.
/// </summary>
public sealed class PasoFlujo : Entity<int>
{
    public int Orden { get; private set; }
    public int RolResponsableId { get; private set; }

    internal PasoFlujo(int orden, int rolResponsableId)
    {
        Orden = orden;
        RolResponsableId = rolResponsableId;
    }

    // Requerido por EF Core
    private PasoFlujo() { }
}
