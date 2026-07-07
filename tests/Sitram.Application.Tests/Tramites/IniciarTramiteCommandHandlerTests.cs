using FluentAssertions;
using Moq;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.IniciarTramite;
using Sitram.Domain.Exceptions;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class IniciarTramiteCommandHandlerTests
{
    private readonly Mock<ITramiteRepository> _repositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private IniciarTramiteCommandHandler CrearHandler() =>
        new(_repositorio.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ComandoValido_CreaTramiteYPersiste()
    {
        // Arrange
        var handler = CrearHandler();
        var comando = new IniciarTramiteCommand(Guid.NewGuid(), TipoTramiteId: 1, "TRA-2026-0001");

        // Act
        var id = await handler.Handle(comando, CancellationToken.None);

        // Assert
        id.Should().NotBe(Guid.Empty);
        _repositorio.Verify(r => r.AddAsync(It.IsAny<Tramite>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CiudadanoVacio_LanzaDomainException_YNoPersiste()
    {
        // Arrange
        var handler = CrearHandler();
        var comando = new IniciarTramiteCommand(Guid.Empty, TipoTramiteId: 1, "TRA-2026-0001");

        // Act
        var act = async () => await handler.Handle(comando, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
