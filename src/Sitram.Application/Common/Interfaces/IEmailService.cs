namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto para el envío de notificaciones por correo (RF-051).</summary>
public interface IEmailService
{
    Task EnviarAsync(string destino, string asunto, string cuerpo, CancellationToken cancellationToken = default);
}
