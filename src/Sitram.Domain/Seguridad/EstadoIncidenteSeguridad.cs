namespace Sitram.Domain.Seguridad;

/// <summary>Ciclo de vida de un incidente de seguridad (RF-065, D.S. 016-2024-JUS).</summary>
public enum EstadoIncidenteSeguridad
{
    Detectado,
    Notificado,
    Resuelto,
}
