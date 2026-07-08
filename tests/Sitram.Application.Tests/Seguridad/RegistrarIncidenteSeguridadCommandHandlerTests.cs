using FluentAssertions;
using Moq;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Seguridad.Commands.RegistrarIncidenteSeguridad;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Seguridad;

namespace Sitram.Application.Tests.Seguridad;

public class RegistrarIncidenteSeguridadCommandHandlerTests
{
    private readonly Mock<IIncidenteSeguridadRepository> _incidenteRepositorio = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RegistrarIncidenteSeguridadCommandHandler CrearHandler() =>
        new(_incidenteRepositorio.Object, _identityService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ConOficialDesignado_NotificaYPersiste()
    {
        var oficial = new UsuarioBasico(Guid.NewGuid(), "oficial", "oficial@sitram.local");
        _identityService.Setup(s => s.ObtenerOficialDatosActivoAsync(It.IsAny<CancellationToken>())).ReturnsAsync(oficial);
        var handler = CrearHandler();

        var resultado = await handler.Handle(
            new RegistrarIncidenteSeguridadCommand("Acceso no autorizado", "Detalle del incidente", "Alta"), CancellationToken.None);

        resultado.Estado.Should().Be("Notificado");
        resultado.OficialNotificado.Should().BeTrue();
        _incidenteRepositorio.Verify(r => r.AddAsync(It.IsAny<IncidenteSeguridad>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SinOficialDesignado_NotificaConOficialNuloYContinua()
    {
        _identityService.Setup(s => s.ObtenerOficialDatosActivoAsync(It.IsAny<CancellationToken>())).ReturnsAsync((UsuarioBasico?)null);
        var handler = CrearHandler();

        var resultado = await handler.Handle(
            new RegistrarIncidenteSeguridadCommand("Fuga de datos", "Detalle", "Critica"), CancellationToken.None);

        resultado.OficialNotificado.Should().BeFalse();
        resultado.Estado.Should().Be("Notificado");
    }

    [Fact]
    public async Task Handle_ConGravedadInvalida_LanzaDomainException()
    {
        var handler = CrearHandler();

        var act = async () => await handler.Handle(
            new RegistrarIncidenteSeguridadCommand("Título", "Detalle", "Extrema"), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}
