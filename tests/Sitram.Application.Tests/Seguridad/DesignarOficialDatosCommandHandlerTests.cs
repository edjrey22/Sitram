using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Seguridad.Commands.DesignarOficialDatos;

namespace Sitram.Application.Tests.Seguridad;

public class DesignarOficialDatosCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IAuditoriaService> _auditoriaService = new();

    private DesignarOficialDatosCommandHandler CrearHandler() =>
        new(_identityService.Object, _auditoriaService.Object);

    [Fact]
    public async Task Handle_UsuarioExistente_DesignaYRegistraAuditoria()
    {
        var usuarioId = Guid.NewGuid();
        _identityService.Setup(s => s.ObtenerUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioBasico(usuarioId, "nuevo.oficial", "oficial@sitram.local"));
        var handler = CrearHandler();

        await handler.Handle(new DesignarOficialDatosCommand(usuarioId), CancellationToken.None);

        _identityService.Verify(s => s.DesignarOficialDatosAsync(usuarioId, It.IsAny<CancellationToken>()), Times.Once);
        _auditoriaService.Verify(a => a.RegistrarAsync(
            null, "OficialDatosDesignado", null, "nuevo.oficial", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UsuarioInexistente_LanzaNotFoundException()
    {
        var handler = CrearHandler();

        var act = async () => await handler.Handle(new DesignarOficialDatosCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
