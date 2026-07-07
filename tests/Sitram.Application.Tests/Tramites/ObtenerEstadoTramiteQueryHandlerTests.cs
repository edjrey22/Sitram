using FluentAssertions;
using Moq;
using Sitram.Application.Tramites.Queries.ObtenerEstadoTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class ObtenerEstadoTramiteQueryHandlerTests
{
    private readonly Mock<ITramiteRepository> _repositorio = new();

    [Fact]
    public async Task Handle_TramiteExistente_DevuelveDtoConEstadoEHistorial()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-0007");
        tramite.Enviar(); // Borrador -> Recibido (registra una actuación)
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<TramiteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tramite);
        var handler = new ObtenerEstadoTramiteQueryHandler(_repositorio.Object);

        // Act
        var dto = await handler.Handle(new ObtenerEstadoTramiteQuery(tramite.Id.Value), CancellationToken.None);

        // Assert
        dto.Should().NotBeNull();
        dto!.Estado.Should().Be("Recibido");
        dto.Codigo.Should().Be("TRA-2026-0007");
        dto.Historial.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_TramiteInexistente_DevuelveNull()
    {
        // Arrange
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<TramiteId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tramite?)null);
        var handler = new ObtenerEstadoTramiteQueryHandler(_repositorio.Object);

        // Act
        var dto = await handler.Handle(new ObtenerEstadoTramiteQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        dto.Should().BeNull();
    }
}
