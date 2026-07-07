namespace Sitram.Domain.Tramites;

/// <summary>Estados del ciclo de vida de un trámite (máquina de estados, ver arquitectura.md §4).</summary>
public enum EstadoTramite
{
    Borrador,
    Recibido,
    EnRevision,
    Observado,
    Aprobado,
    Rechazado,
}
