using System.Text.Json;
using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Application.Tramites.Events;

/// <summary>
/// Registra en auditoría cada transición de estado del trámite (RF-070), con el estado anterior
/// y el nuevo. No incluye datos personales: solo el nombre del estado y el comentario operativo.
/// </summary>
public sealed class TramiteEstadoCambiadoEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<TramiteEstadoCambiadoEvent>>
{
    public Task Handle(DomainEventNotification<TramiteEstadoCambiadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        return auditoriaService.RegistrarAsync(
            evento.TramiteId.Value,
            accion: $"{evento.EstadoAnterior}->{evento.EstadoNuevo}",
            datosAntes: JsonSerializer.Serialize(new { estado = evento.EstadoAnterior.ToString() }),
            datosDespues: JsonSerializer.Serialize(new { estado = evento.EstadoNuevo.ToString(), comentario = evento.Comentario }),
            cancellationToken);
    }
}
