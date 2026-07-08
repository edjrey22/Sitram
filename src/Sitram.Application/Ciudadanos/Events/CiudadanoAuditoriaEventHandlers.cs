using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos.Events;

namespace Sitram.Application.Ciudadanos.Events;

/// <summary>
/// Traduce los eventos del agregado <see cref="Domain.Ciudadanos.Ciudadano"/> a auditoría
/// (RF-070). Nunca se incluyen datos personales en <c>datosAntes/datosDespues</c> (RNF-010):
/// solo se registra la acción y el identificador.
/// </summary>
public sealed class CiudadanoRegistradoEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<CiudadanoRegistradoEvent>>
{
    public Task Handle(DomainEventNotification<CiudadanoRegistradoEvent> notification, CancellationToken cancellationToken) =>
        auditoriaService.RegistrarAsync(null, "CiudadanoRegistrado", null, null, cancellationToken);
}

public sealed class DatosCiudadanoRectificadosEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<DatosCiudadanoRectificadosEvent>>
{
    public Task Handle(DomainEventNotification<DatosCiudadanoRectificadosEvent> notification, CancellationToken cancellationToken) =>
        auditoriaService.RegistrarAsync(null, "DatosCiudadanoRectificados", null, null, cancellationToken);
}

public sealed class CiudadanoAnonimizadoEventHandler(IAuditoriaService auditoriaService)
    : INotificationHandler<DomainEventNotification<CiudadanoAnonimizadoEvent>>
{
    public Task Handle(DomainEventNotification<CiudadanoAnonimizadoEvent> notification, CancellationToken cancellationToken) =>
        auditoriaService.RegistrarAsync(null, "CiudadanoAnonimizado", null, null, cancellationToken);
}
