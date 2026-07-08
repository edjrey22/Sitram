using FluentAssertions;
using Moq;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;
using Sitram.Application.Tramites.Commands.IniciarTramite;
using Sitram.Domain.Exceptions;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;

namespace Sitram.Application.Tests.Tramites;

public class IniciarTramiteCommandHandlerTests
{
    private readonly Mock<ITramiteRepository> _repositorio = new();
    private readonly Mock<ITipoTramiteRepository> _tipoTramiteRepositorio = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private IniciarTramiteCommandHandler CrearHandler() =>
        new(_repositorio.Object, _tipoTramiteRepositorio.Object, _unitOfWork.Object);

    private void ConfigurarTipoTramiteActivo(int id = 1)
    {
        var tipoTramite = TipoTramite.Crear("Licencia de funcionamiento", "Descripción", "Rentas", 150m);
        _tipoTramiteRepositorio
            .Setup(r => r.ObtenerPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tipoTramite);
    }

    [Fact]
    public async Task Handle_ComandoValido_CreaTramiteYPersiste()
    {
        // Arrange
        ConfigurarTipoTramiteActivo();
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
        ConfigurarTipoTramiteActivo();
        var handler = CrearHandler();
        var comando = new IniciarTramiteCommand(Guid.Empty, TipoTramiteId: 1, "TRA-2026-0001");

        // Act
        var act = async () => await handler.Handle(comando, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _unitOfWork.Verify(u => u.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TipoTramiteInexistente_LanzaTipoTramiteNoDisponibleException()
    {
        // Arrange: el mock no configurado devuelve null (tipo de trámite inexistente)
        var handler = CrearHandler();
        var comando = new IniciarTramiteCommand(Guid.NewGuid(), TipoTramiteId: 99, "TRA-2026-0002");

        // Act
        var act = async () => await handler.Handle(comando, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TipoTramiteNoDisponibleException>();
        _repositorio.Verify(r => r.AddAsync(It.IsAny<Tramite>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TipoTramiteDesactivado_LanzaTipoTramiteNoDisponibleException()
    {
        // Arrange
        var tipoTramite = TipoTramite.Crear("Certificado", "Descripción", "Rentas", 50m);
        tipoTramite.Desactivar();
        _tipoTramiteRepositorio
            .Setup(r => r.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tipoTramite);
        var handler = CrearHandler();
        var comando = new IniciarTramiteCommand(Guid.NewGuid(), TipoTramiteId: 1, "TRA-2026-0003");

        // Act
        var act = async () => await handler.Handle(comando, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TipoTramiteNoDisponibleException>();
    }
}
