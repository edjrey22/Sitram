using FluentAssertions;
using Moq;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.EnviarAlertaVencimiento;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class EnviarAlertaVencimientoCommandHandlerTests
{
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<ICiudadanoRepository> _ciudadanoRepositorio = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private EnviarAlertaVencimientoCommandHandler CrearHandler() =>
        new(_tramiteRepositorio.Object, _ciudadanoRepositorio.Object, _emailService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_TramiteObservado_EnviaCorreoYMarcaAlertaEnviada()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-6300");
        tramite.Enviar();
        tramite.IniciarRevision();
        tramite.Observar("Falta documento");
        var ciudadano = Ciudadano.Registrar(
            tramite.CiudadanoId, "Ana", "Pérez", "12345678", "ana@correo.com", "987654321", "Av. Test 123");
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        _ciudadanoRepositorio
            .Setup(r => r.ObtenerPorIdAsync(new CiudadanoId(tramite.CiudadanoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ciudadano);
        var handler = CrearHandler();

        // Act
        await handler.Handle(new EnviarAlertaVencimientoCommand(tramite.Id.Value), CancellationToken.None);

        // Assert
        _emailService.Verify(s => s.EnviarAsync(
            "ana@correo.com", It.Is<string>(a => a.Contains("por vencer")), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        tramite.AlertaVencimientoEnviada.Should().BeTrue();
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
