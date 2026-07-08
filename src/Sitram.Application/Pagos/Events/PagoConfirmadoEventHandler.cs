using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Pagos.Events;

namespace Sitram.Application.Pagos.Events;

/// <summary>Registra en auditoría la confirmación de un pago (RF-070).</summary>
public sealed class PagoConfirmadoEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<PagoConfirmadoEvent>>
{
    public Task Handle(DomainEventNotification<PagoConfirmadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        return auditoriaService.RegistrarAsync(
            evento.TramiteId.Value, "PagoConfirmado", null, null, cancellationToken);
    }
}
