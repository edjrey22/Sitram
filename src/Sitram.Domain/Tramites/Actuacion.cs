using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites;

/// <summary>
/// Entidad hija del agregado <see cref="Tramite"/>: registra una transición de estado del
/// expediente (historial de actuaciones, RF-052). Se crea solo desde el agregado.
/// </summary>
public sealed class Actuacion : Entity<Guid>
{
    public EstadoTramite EstadoAnterior { get; private set; }
    public EstadoTramite EstadoNuevo { get; private set; }
    public string? Comentario { get; private set; }
    public DateTime FechaUtc { get; private set; }

    internal Actuacion(EstadoTramite estadoAnterior, EstadoTramite estadoNuevo, string? comentario)
        : base(Guid.NewGuid())
    {
        EstadoAnterior = estadoAnterior;
        EstadoNuevo = estadoNuevo;
        Comentario = comentario;
        FechaUtc = DateTime.UtcNow;
    }

    // Requerido por EF Core
    private Actuacion() { }
}
