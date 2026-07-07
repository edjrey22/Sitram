using FluentAssertions;
using Moq;
using Sitram.Application.Auth.Commands.IniciarSesion;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Tests.Auth;

public class IniciarSesionCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<IRefreshTokenService> _refreshTokenService = new();

    private IniciarSesionCommandHandler CrearHandler() =>
        new(_identityService.Object, _jwtTokenService.Object, _refreshTokenService.Object);

    [Fact]
    public async Task Handle_CredencialesValidas_DevuelveTokens()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _identityService
            .Setup(s => s.ValidarCredencialesAsync("ana", "clave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidarCredencialesResultado(true, false, usuarioId));
        _identityService
            .Setup(s => s.ObtenerPermisosAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(["tramite:iniciar"]);
        _jwtTokenService
            .Setup(s => s.GenerarAccessToken(usuarioId, "ana", It.IsAny<IEnumerable<string>>()))
            .Returns(("token-jwt", DateTime.UtcNow.AddMinutes(15)));
        _refreshTokenService
            .Setup(s => s.EmitirAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("refresh-token");
        var handler = CrearHandler();

        // Act
        var resultado = await handler.Handle(new IniciarSesionCommand("ana", "clave"), CancellationToken.None);

        // Assert
        resultado.AccessToken.Should().Be("token-jwt");
        resultado.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_CredencialesInvalidas_LanzaAutenticacionInvalidaException()
    {
        // Arrange
        _identityService
            .Setup(s => s.ValidarCredencialesAsync("ana", "mala", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidarCredencialesResultado(false, false, Guid.Empty));
        var handler = CrearHandler();

        // Act
        var act = async () => await handler.Handle(new IniciarSesionCommand("ana", "mala"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AutenticacionInvalidaException>();
    }

    [Fact]
    public async Task Handle_CuentaBloqueada_LanzaAutenticacionInvalidaException()
    {
        // Arrange
        _identityService
            .Setup(s => s.ValidarCredencialesAsync("ana", "clave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidarCredencialesResultado(false, true, Guid.NewGuid()));
        var handler = CrearHandler();

        // Act
        var act = async () => await handler.Handle(new IniciarSesionCommand("ana", "clave"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AutenticacionInvalidaException>();
    }
}
