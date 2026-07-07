using System.Text.Json;
using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Application.Tramites.Events;

/// <summary>Registra en auditoría la creación de un trámite (RF-070).</summary>
public sealed class TramiteCreadoEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<TramiteCreadoEvent>>
{
    public Task Handle(DomainEventNotification<TramiteCreadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        return auditoriaService.RegistrarAsync(
            evento.TramiteId.Value,
            accion: "TramiteCreado",
            datosAntes: null,
            datosDespues: JsonSerializer.Serialize(new { estado = "Borrador" }),
            cancellationToken);
    }
}
