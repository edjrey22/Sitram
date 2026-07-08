using FluentAssertions;
using Moq;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Events;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Tramites;
using Sitram.Domain.Tramites.Events;

namespace Sitram.Application.Tests.Tramites;

public class NotificacionTramiteEventHandlersTests
{
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<ICiudadanoRepository> _ciudadanoRepositorio = new();
    private readonly Mock<IEmailService> _emailService = new();

    private Tramite ConfigurarTramiteYCiudadano(out Ciudadano ciudadano)
    {
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-7000");
        ciudadano = Ciudadano.Registrar(
            tramite.CiudadanoId, "Ana", "Pérez", "12345678", "ana@correo.com", "987654321", "Av. Test 123");
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        _ciudadanoRepositorio
            .Setup(r => r.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ciudadano);
        return tramite;
    }

    [Fact]
    public async Task TramiteObservadoEventHandler_EnviaCorreoAlCiudadano()
    {
        var tramite = ConfigurarTramiteYCiudadano(out _);
        var handler = new TramiteObservadoEventHandler(_tramiteRepositorio.Object, _ciudadanoRepositorio.Object, _emailService.Object);

        await handler.Handle(
            new DomainEventNotification<TramiteObservadoEvent>(new TramiteObservadoEvent(tramite.Id, "Falta documento")),
            CancellationToken.None);

        _emailService.Verify(s => s.EnviarAsync(
            "ana@correo.com", It.Is<string>(a => a.Contains("subsanación")), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TramiteAprobadoEventHandler_EnviaCorreoAlCiudadano()
    {
        var tramite = ConfigurarTramiteYCiudadano(out _);
        var handler = new TramiteAprobadoEventHandler(_tramiteRepositorio.Object, _ciudadanoRepositorio.Object, _emailService.Object);

        await handler.Handle(
            new DomainEventNotification<TramiteAprobadoEvent>(new TramiteAprobadoEvent(tramite.Id)),
            CancellationToken.None);

        _emailService.Verify(s => s.EnviarAsync(
            "ana@correo.com", It.Is<string>(a => a.Contains("aprobado")), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TramiteRechazadoEventHandler_NoEnviaCorreoSiElCiudadanoEstaAnonimizado()
    {
        var tramite = ConfigurarTramiteYCiudadano(out var ciudadano);
        ciudadano.Anonimizar();
        var handler = new TramiteRechazadoEventHandler(_tramiteRepositorio.Object, _ciudadanoRepositorio.Object, _emailService.Object);

        await handler.Handle(
            new DomainEventNotification<TramiteRechazadoEvent>(new TramiteRechazadoEvent(tramite.Id, "Motivo")),
            CancellationToken.None);

        _emailService.Verify(s => s.EnviarAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
