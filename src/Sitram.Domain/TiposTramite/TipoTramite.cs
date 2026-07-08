using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;

namespace Sitram.Domain.TiposTramite;

/// <summary>
/// Agregado raíz: plantilla de un tipo de trámite del TUPA (RF-010…014). Su identificador es
/// un <c>int</c> autogenerado por la base de datos (modelo-datos.md), a diferencia de
/// <c>TramiteId</c> (Guid generado en cliente).
/// </summary>
public sealed class TipoTramite : AggregateRoot<int>
{
    private readonly List<RequisitoDocumento> _requisitos = new();
    private readonly List<PasoFlujo> _pasosFlujo = new();

    public string Nombre { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;
    public string AreaResponsable { get; private set; } = default!;
    public decimal Tasa { get; private set; }

    /// <summary>Borrado lógico (RF-013): nunca se elimina un tipo de trámite, solo se desactiva.</summary>
    public bool Activo { get; private set; }

    public IReadOnlyCollection<RequisitoDocumento> Requisitos => _requisitos.AsReadOnly();
    public IReadOnlyCollection<PasoFlujo> PasosFlujo => _pasosFlujo.AsReadOnly();

    // Requerido por EF Core
    private TipoTramite() { }

    private TipoTramite(string nombre, string descripcion, string areaResponsable, decimal tasa)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        AreaResponsable = areaResponsable;
        Tasa = tasa;
        Activo = true;
    }

    /// <summary>Crea un tipo de trámite nuevo y activo (RF-010).</summary>
    public static TipoTramite Crear(string nombre, string descripcion, string areaResponsable, decimal tasa)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(areaResponsable);
        if (tasa < 0)
            throw new DomainException("La tasa no puede ser negativa.");

        return new TipoTramite(nombre.Trim(), descripcion?.Trim() ?? string.Empty, areaResponsable.Trim(), tasa);
    }

    /// <summary>Añade un documento requerido para este tipo de trámite (RF-011).</summary>
    public void AgregarRequisito(string nombre, bool obligatorio)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        _requisitos.Add(new RequisitoDocumento(nombre.Trim(), obligatorio));
    }

    /// <summary>Añade un paso al flujo de aprobación (RF-012); el orden no puede repetirse.</summary>
    public void AgregarPasoFlujo(int orden, int rolResponsableId)
    {
        if (orden <= 0)
            throw new DomainException("El orden del paso debe ser mayor a cero.");
        if (_pasosFlujo.Any(p => p.Orden == orden))
            throw new DomainException($"Ya existe un paso de flujo con el orden {orden}.");

        _pasosFlujo.Add(new PasoFlujo(orden, rolResponsableId));
    }

    /// <summary>Reactiva el tipo de trámite (RF-013).</summary>
    public void Activar() => Activo = true;

    /// <summary>Desactiva el tipo de trámite sin borrarlo (RF-013).</summary>
    public void Desactivar() => Activo = false;
}
