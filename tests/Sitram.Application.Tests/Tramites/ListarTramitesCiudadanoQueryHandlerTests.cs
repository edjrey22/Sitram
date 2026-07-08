using FluentAssertions;
using Moq;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;
using Sitram.Application.Tramites.Queries.ListarTramitesCiudadano;

namespace Sitram.Application.Tests.Tramites;

public class ListarTramitesCiudadanoQueryHandlerTests
{
    [Fact]
    public async Task Handle_DelegaEnElServicioDeLectura_ConLosParametrosRecibidos()
    {
        // Arrange
        var ciudadanoId = Guid.NewGuid();
        var esperado = new PagedResult<TramiteResumenDto>(
            [new TramiteResumenDto(Guid.NewGuid(), "TRA-2026-0001", "Borrador", "Licencia de funcionamiento", DateTime.UtcNow)],
            TotalCount: 1, Page: 1, PageSize: 20);

        var readService = new Mock<ITramitesReadService>();
        readService
            .Setup(s => s.ListarPorCiudadanoAsync(ciudadanoId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);
        var handler = new ListarTramitesCiudadanoQueryHandler(readService.Object);

        // Act
        var resultado = await handler.Handle(
            new ListarTramitesCiudadanoQuery(ciudadanoId, 1, 20), CancellationToken.None);

        // Assert
        resultado.Should().BeSameAs(esperado);
        readService.Verify(
            s => s.ListarPorCiudadanoAsync(ciudadanoId, 1, 20, It.IsAny<CancellationToken>()), Times.Once);
    }
}
