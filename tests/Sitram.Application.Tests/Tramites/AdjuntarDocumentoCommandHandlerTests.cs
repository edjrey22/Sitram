using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.AdjuntarDocumento;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class AdjuntarDocumentoCommandHandlerTests
{
    private readonly Mock<ITramiteRepository> _tramiteRepositorio = new();
    private readonly Mock<IAlmacenamientoArchivos> _almacenamiento = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AdjuntarDocumentoCommandHandler CrearHandler() =>
        new(_tramiteRepositorio.Object, _almacenamiento.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ArchivoValido_LoGuardaYPersiste()
    {
        // Arrange
        var tramite = Tramite.Crear(Guid.NewGuid(), tipoTramiteId: 1, "TRA-2026-6100");
        _tramiteRepositorio.Setup(r => r.ObtenerPorIdAsync(tramite.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tramite);
        _almacenamiento
            .Setup(a => a.GuardarAsync("recibo.pdf", It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("ruta-fisica.pdf", "hash-abc"));
        using var contenido = new MemoryStream([1, 2, 3]);
        var handler = CrearHandler();

        // Act
        await handler.Handle(new AdjuntarDocumentoCommand(tramite.Id.Value, "recibo.pdf", contenido), CancellationToken.None);

        // Assert
        tramite.Documentos.Should().ContainSingle(d => d.HashSha256 == "hash-abc");
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExtensionNoPermitida_LanzaDomainException_SinGuardarEnDisco()
    {
        // Arrange
        using var contenido = new MemoryStream([1, 2, 3]);
        var handler = CrearHandler();

        // Act
        var act = async () =>
            await handler.Handle(new AdjuntarDocumentoCommand(Guid.NewGuid(), "virus.exe", contenido), CancellationToken.None);

        // Assert: rechazado antes de tocar el almacenamiento (evita un archivo huérfano)
        await act.Should().ThrowAsync<DomainException>();
        _almacenamiento.Verify(
            a => a.GuardarAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TramiteInexistente_LanzaNotFoundException()
    {
        // Arrange
        _almacenamiento
            .Setup(a => a.GuardarAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("ruta", "hash"));
        using var contenido = new MemoryStream([1, 2, 3]);
        var handler = CrearHandler();

        // Act
        var act = async () =>
            await handler.Handle(new AdjuntarDocumentoCommand(Guid.NewGuid(), "recibo.pdf", contenido), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
