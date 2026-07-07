using MediatR;
using Sitram.Domain.Common;

namespace Sitram.Application.Common.Events;

/// <summary>
/// Adapta un <see cref="IDomainEvent"/> (marcador puro del dominio, sin dependencias) a una
/// notificación de MediatR, para poder despacharlo con <c>IPublisher</c> tras guardar cambios.
/// </summary>
public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}
