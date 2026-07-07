using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.Transiciones;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class TransicionesTramiteTests
{
    private readonly Mock<ITramiteRepository> _repositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Enviar_TramiteInexistente_LanzaNotFoundException_YNoPersiste()
    {
        // Arrange
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<TramiteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tramite?)null);
        var handler = new EnviarTramiteCommandHandler(_repositorio.Object, _unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(new EnviarTramiteCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Enviar_TramiteEnBorrador_CambiaARecibido_YPersiste()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0100");
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<TramiteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tramite);
        var handler = new EnviarTramiteCommandHandler(_repositorio.Object, _unitOfWork.Object);

        // Act
        await handler.Handle(new EnviarTramiteCommand(tramite.Id.Value), CancellationToken.None);

        // Assert
        tramite.Estado.Should().Be(EstadoTramite.Recibido);
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
