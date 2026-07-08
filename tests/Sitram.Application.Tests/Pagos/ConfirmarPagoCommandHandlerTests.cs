using FluentAssertions;
using Moq;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Pagos.Commands.ConfirmarPago;
using Sitram.Domain.Pagos;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Pagos;

public class ConfirmarPagoCommandHandlerTests
{
    private readonly Mock<IPagoRepository> _pagoRepositorio = new();
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ConfirmarPagoCommandHandler CrearHandler() =>
        new(_pagoRepositorio.Object, _tramiteRepositorio.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_PagoPendiente_LoConfirmaYAvanzaElTramiteEnLaMismaOperacion()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-8100");
        tramite.Enviar(); // Borrador -> Recibido (precondición de IniciarRevision)
        var pago = Pago.Registrar(tramite.Id, 150m);
        _pagoRepositorio.Setup(r => r.ObtenerPorIdAsync(pago.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pago);
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        var handler = CrearHandler();

        // Act
        await handler.Handle(new ConfirmarPagoCommand(pago.Id.Value), CancellationToken.None);

        // Assert: RNF-032 — pago confirmado y trámite avanzado, persistidos en una sola llamada
        pago.Estado.Should().Be(EstadoPago.Confirmado);
        tramite.Estado.Should().Be(EstadoTramite.EnRevision);
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
