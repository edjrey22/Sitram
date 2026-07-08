using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Seguridad.Events;

namespace Sitram.Application.Seguridad.Events;

/// <summary>
/// Envía la notificación por correo al Oficial de Datos Personales y deja constancia en
/// auditoría (RF-065, D.S. 016-2024-JUS).
/// </summary>
public sealed class IncidenteSeguridadNotificadoEventHandler(
    IIdentityService identityService, IEmailService emailService, IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<IncidenteSeguridadNotificadoEvent>>
{
    public async Task Handle(DomainEventNotification<IncidenteSeguridadNotificadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;

        if (evento.OficialNotificadoId is Guid oficialId)
        {
            var oficial = await identityService.ObtenerUsuarioAsync(oficialId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(oficial?.Email))
            {
                await emailService.EnviarAsync(
                    oficial.Email, $"Incidente de seguridad detectado: {evento.Titulo}",
                    $"Se detectó un incidente de seguridad de gravedad {evento.Gravedad}: {evento.Titulo}. " +
                    "Ingresa a SITRAM para revisar los detalles y coordinar la respuesta (D.S. 016-2024-JUS).",
                    cancellationToken);
            }
        }

        await auditoriaService.RegistrarAsync(
            null, "IncidenteSeguridadNotificado", null,
            $"{{\"incidenteId\":\"{evento.IncidenteId.Value}\",\"gravedad\":\"{evento.Gravedad}\"}}", cancellationToken);
    }
}
