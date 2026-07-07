namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Registro inmutable de auditoría (EVENTO_AUDITORIA en modelo-datos.md). Append-only: no existe
/// ningún método de actualización ni borrado en <c>IAuditoriaService</c> (RF-073).
/// </summary>
public sealed class EventoAuditoria
{
    public long EventoId { get; set; }
    public Guid? TramiteId { get; set; }
    public Guid? UsuarioId { get; set; }
    public string Accion { get; set; } = default!;
    public string? DatosAntes { get; set; }
    public string? DatosDespues { get; set; }
    public string? DireccionIp { get; set; }
    public DateTime FechaUtc { get; set; }
}
