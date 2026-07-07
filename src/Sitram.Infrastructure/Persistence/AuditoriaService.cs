using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación de <see cref="IAuditoriaService"/>: inserta el evento en la misma sesión de EF Core.</summary>
public sealed class AuditoriaService(SitramDbContext context, ICurrentUserService currentUserService) : IAuditoriaService
{
    public async Task RegistrarAsync(
        Guid? tramiteId, string accion, string? datosAntes, string? datosDespues,
        CancellationToken cancellationToken = default)
    {
        context.EventosAuditoria.Add(new EventoAuditoria
        {
            TramiteId = tramiteId,
            UsuarioId = currentUserService.UsuarioId,
            Accion = accion,
            DatosAntes = datosAntes,
            DatosDespues = datosDespues,
            DireccionIp = currentUserService.DireccionIp,
            FechaUtc = DateTime.UtcNow,
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
