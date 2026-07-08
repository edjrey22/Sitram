using Sitram.Domain.Common;

namespace Sitram.Domain.TiposTramite;

/// <summary>Documento exigido por un tipo de trámite (RF-011). Se crea solo desde el agregado.</summary>
public sealed class RequisitoDocumento : Entity<int>
{
    public string Nombre { get; private set; } = default!;
    public bool Obligatorio { get; private set; }

    internal RequisitoDocumento(string nombre, bool obligatorio)
    {
        Nombre = nombre;
        Obligatorio = obligatorio;
    }

    // Requerido por EF Core
    private RequisitoDocumento() { }
}
