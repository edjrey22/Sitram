using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.Transiciones;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

/// <summary>RF-043: impedir el avance de un trámite con tasa impaga.</summary>
public class IniciarRevisionPuertaDePagoTests
{
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<ITipoTramiteRepository> _tipoTramiteRepositorio = new();
    private readonly Mock<IPagoRepository> _pagoRepositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private IniciarRevisionTramiteCommandHandler CrearHandler() =>
        new(_tramiteRepositorio.Object, _tipoTramiteRepositorio.Object, _pagoRepositorio.Object, _unitOfWork.Object);

    private Tramite CrearTramiteRecibido(int tipoTramiteId = 1)
    {
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId, "TRA-2026-8200");
        tramite.Enviar();
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        return tramite;
    }

    [Fact]
    public async Task Handle_TipoTramiteConTasaYSinPagoConfirmado_LanzaPagoRequeridoException()
    {
        // Arrange
        var tramite = CrearTramiteRecibido();
        var tipoTramite = TipoTramite.Crear("Licencia", "Desc", "Rentas", 150m);
        _tipoTramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tipoTramite);
        _pagoRepositorio.Setup(r => r.ObtenerPorTramiteAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Pago?)null);
        var handler = CrearHandler();

        // Act
        var act = async () => await handler.Handle(new IniciarRevisionTramiteCommand(tramite.Id.Value), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<PagoRequeridoException>();
        tramite.Estado.Should().Be(EstadoTramite.Recibido); // no avanzó
    }

    [Fact]
    public async Task Handle_TipoTramiteConTasaYPagoConfirmado_Avanza()
    {
        // Arrange
        var tramite = CrearTramiteRecibido();
        var tipoTramite = TipoTramite.Crear("Licencia", "Desc", "Rentas", 150m);
        var pago = Pago.Registrar(tramite.Id, 150m);
        pago.Confirmar();
        _tipoTramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tipoTramite);
        _pagoRepositorio.Setup(r => r.ObtenerPorTramiteAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pago);
        var handler = CrearHandler();

        // Act
        await handler.Handle(new IniciarRevisionTramiteCommand(tramite.Id.Value), CancellationToken.None);

        // Assert
        tramite.Estado.Should().Be(EstadoTramite.EnRevision);
    }

    [Fact]
    public async Task Handle_TipoTramiteGratuito_NoRequierePago()
    {
        // Arrange
        var tramite = CrearTramiteRecibido();
        var tipoGratuito = TipoTramite.Crear("Constancia gratuita", "Desc", "Rentas", 0m);
        _tipoTramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tipoGratuito);
        var handler = CrearHandler();

        // Act
        await handler.Handle(new IniciarRevisionTramiteCommand(tramite.Id.Value), CancellationToken.None);

        // Assert: avanza sin necesidad de consultar el repositorio de pagos
        tramite.Estado.Should().Be(EstadoTramite.EnRevision);
        _pagoRepositorio.Verify(r => r.ObtenerPorTramiteAsync(It.IsAny<TramiteId>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
