using FluentAssertions;
using Moq;
using Sitram.Application.Auditoria.Queries.ConsultarAuditoria;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Common.Models;

namespace Sitram.Application.Tests.Auditoria;

public class ConsultarAuditoriaQueryHandlerTests
{
    private readonly Mock<IAuditoriaReadService> _readService = new();

    [Fact]
    public async Task Handle_DelegaLosFiltrosYLaPaginacionAlServicioDeLectura()
    {
        var usuarioId = Guid.NewGuid();
        var desde = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var hasta = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var esperado = new PagedResult<EventoAuditoriaDetalleDto>(
            [new EventoAuditoriaDetalleDto(1, null, usuarioId, "PagoConfirmado", "127.0.0.1", DateTime.UtcNow)], 1, 1, 20);

        _readService
            .Setup(s => s.ConsultarAsync(usuarioId, "PagoConfirmado", desde, hasta, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(esperado);

        var handler = new ConsultarAuditoriaQueryHandler(_readService.Object);

        var resultado = await handler.Handle(
            new ConsultarAuditoriaQuery(usuarioId, "PagoConfirmado", desde, hasta, 1, 20), CancellationToken.None);

        resultado.Should().BeSameAs(esperado);
    }
}
