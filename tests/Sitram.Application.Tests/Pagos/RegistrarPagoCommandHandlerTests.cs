using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Pagos.Commands.RegistrarPago;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Pagos;

public class RegistrarPagoCommandHandlerTests
{
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<ITipoTramiteRepository> _tipoTramiteRepositorio = new();
    private readonly Mock<IPagoRepository> _pagoRepositorio = new();
    private readonly Mock<IPagoService> _pagoService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RegistrarPagoCommandHandler CrearHandler() =>
        new(_tramiteRepositorio.Object, _tipoTramiteRepositorio.Object, _pagoRepositorio.Object, _pagoService.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_TramiteYTipoValidos_CalculaMontoDesdeLaTasaYPersiste()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-8000");
        var tipoTramite = TipoTramite.Crear("Licencia", "Desc", "Rentas", 150m);
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        _tipoTramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tipoTramite);
        _pagoService
            .Setup(s => s.RegistrarPagoAsync(150m, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("REF-123");
        var handler = CrearHandler();

        // Act
        var resultado = await handler.Handle(new RegistrarPagoCommand(tramite.Id.Value), CancellationToken.None);

        // Assert
        resultado.Monto.Should().Be(150m);
        resultado.ReferenciaPasarela.Should().Be("REF-123");
        _pagoRepositorio.Verify(r => r.AddAsync(It.IsAny<Pago>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TramiteInexistente_LanzaNotFoundException()
    {
        var handler = CrearHandler();

        var act = async () => await handler.Handle(new RegistrarPagoCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
