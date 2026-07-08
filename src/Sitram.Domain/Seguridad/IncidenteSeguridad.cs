using Sitram.Domain.Common;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Seguridad.Events;

namespace Sitram.Domain.Seguridad;

/// <summary>
/// Agregado raíz: incidente de seguridad (brecha de datos personales) que el sistema detecta y
/// notifica al Oficial de Datos Personales, dejando constancia (RF-065, D.S. 016-2024-JUS).
/// </summary>
public sealed class IncidenteSeguridad : AggregateRoot<IncidenteSeguridadId>
{
    public string Titulo { get; private set; } = default!;
    public string Descripcion { get; private set; } = default!;
    public GravedadIncidente Gravedad { get; private set; }
    public EstadoIncidenteSeguridad Estado { get; private set; }
    public DateTime FechaDeteccionUtc { get; private set; }
    public DateTime? FechaNotificacionUtc { get; private set; }
    public Guid? OficialNotificadoId { get; private set; }
    public string? Resolucion { get; private set; }
    public DateTime? FechaResolucionUtc { get; private set; }

    // Requerido por EF Core
    private IncidenteSeguridad() { }

    private IncidenteSeguridad(IncidenteSeguridadId id, string titulo, string descripcion, GravedadIncidente gravedad)
        : base(id)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Gravedad = gravedad;
        Estado = EstadoIncidenteSeguridad.Detectado;
        FechaDeteccionUtc = DateTime.UtcNow;
    }

    /// <summary>Registra la detección de un incidente de seguridad.</summary>
    public static IncidenteSeguridad Detectar(string titulo, string descripcion, GravedadIncidente gravedad)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        ArgumentException.ThrowIfNullOrWhiteSpace(descripcion);

        return new IncidenteSeguridad(IncidenteSeguridadId.New(), titulo.Trim(), descripcion.Trim(), gravedad);
    }

    /// <summary>
    /// Notifica el incidente al Oficial de Datos Personales activo y deja constancia (RF-065).
    /// <paramref name="oficialNotificadoId"/> puede ser nulo si aún no hay ningún oficial designado.
    /// </summary>
    public void Notificar(Guid? oficialNotificadoId)
    {
        if (Estado != EstadoIncidenteSeguridad.Detectado)
            throw new DomainException("Solo un incidente detectado puede notificarse.");

        Estado = EstadoIncidenteSeguridad.Notificado;
        FechaNotificacionUtc = DateTime.UtcNow;
        OficialNotificadoId = oficialNotificadoId;
        RaiseDomainEvent(new IncidenteSeguridadNotificadoEvent(Id, oficialNotificadoId, Titulo, Gravedad));
    }

    /// <summary>Cierra el incidente con la resolución aplicada; solo procede tras haberse notificado.</summary>
    public void Resolver(string resolucion)
    {
        if (Estado != EstadoIncidenteSeguridad.Notificado)
            throw new DomainException("Solo un incidente notificado puede resolverse.");

        ArgumentException.ThrowIfNullOrWhiteSpace(resolucion);
        Estado = EstadoIncidenteSeguridad.Resuelto;
        Resolucion = resolucion.Trim();
        FechaResolucionUtc = DateTime.UtcNow;
    }
}
