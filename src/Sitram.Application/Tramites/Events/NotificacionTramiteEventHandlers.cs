using MediatR;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Tramites;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Application.Tramites.Events;

/// <summary>
/// Notifica por correo los cambios de estado relevantes para el ciudadano (RF-051). Estos
/// eventos específicos existían desde el Sprint del dominio pero no tenían ningún handler:
/// se generaban y se perdían, igual que pasó con la auditoría antes de conectarla.
/// </summary>
public sealed class TramiteObservadoEventHandler(
    ITramiteRepository tramiteRepositorio, ICiudadanoRepository ciudadanoRepositorio, IEmailService emailService)
    : INotificationHandler<DomainEventNotification<TramiteObservadoEvent>>
{
    public async Task Handle(DomainEventNotification<TramiteObservadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(evento.TramiteId, cancellationToken);
        if (tramite is null) return;

        var ciudadano = await ciudadanoRepositorio.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), cancellationToken);
        if (ciudadano is null || ciudadano.EstaAnonimizado) return;

        await emailService.EnviarAsync(
            ciudadano.Correo, $"Tu trámite {tramite.Codigo} requiere subsanación",
            $"Estimado(a) {ciudadano.Nombres}, tu trámite {tramite.Codigo} fue observado. " +
            $"Motivo: {evento.Motivo}. Ingresa a SITRAM para subsanarlo.", cancellationToken);
    }
}

public sealed class TramiteAprobadoEventHandler(
    ITramiteRepository tramiteRepositorio, ICiudadanoRepository ciudadanoRepositorio, IEmailService emailService)
    : INotificationHandler<DomainEventNotification<TramiteAprobadoEvent>>
{
    public async Task Handle(DomainEventNotification<TramiteAprobadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(evento.TramiteId, cancellationToken);
        if (tramite is null) return;

        var ciudadano = await ciudadanoRepositorio.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), cancellationToken);
        if (ciudadano is null || ciudadano.EstaAnonimizado) return;

        await emailService.EnviarAsync(
            ciudadano.Correo, $"Tu trámite {tramite.Codigo} fue aprobado",
            $"Estimado(a) {ciudadano.Nombres}, tu trámite {tramite.Codigo} fue aprobado. Ingresa a SITRAM para más detalles.",
            cancellationToken);
    }
}

public sealed class TramiteRechazadoEventHandler(
    ITramiteRepository tramiteRepositorio, ICiudadanoRepository ciudadanoRepositorio, IEmailService emailService)
    : INotificationHandler<DomainEventNotification<TramiteRechazadoEvent>>
{
    public async Task Handle(DomainEventNotification<TramiteRechazadoEvent> notification, CancellationToken cancellationToken)
    {
        var evento = notification.DomainEvent;
        var tramite = await tramiteRepositorio.ObtenerPorIdAsync(evento.TramiteId, cancellationToken);
        if (tramite is null) return;

        var ciudadano = await ciudadanoRepositorio.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), cancellationToken);
        if (ciudadano is null || ciudadano.EstaAnonimizado) return;

        await emailService.EnviarAsync(
            ciudadano.Correo, $"Tu trámite {tramite.Codigo} fue rechazado",
            $"Estimado(a) {ciudadano.Nombres}, tu trámite {tramite.Codigo} fue rechazado. " +
            $"Motivo: {evento.Motivo}. Ingresa a SITRAM para más detalles.", cancellationToken);
    }
}
