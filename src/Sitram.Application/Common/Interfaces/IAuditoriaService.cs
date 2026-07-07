namespace Sitram.Application.Common.Interfaces;

/// <summary>
/// Puerto de escritura del registro de auditoría (RF-070). Deliberadamente **solo expone
/// inserción**: no hay método de edición ni borrado, para que la inmutabilidad (RF-073) sea
/// estructural desde la capa de aplicación, no solo una convención de la base de datos.
/// </summary>
public interface IAuditoriaService
{
    Task RegistrarAsync(
        Guid? tramiteId, string accion, string? datosAntes, string? datosDespues,
        CancellationToken cancellationToken = default);
}
