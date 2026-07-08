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
    private readonly Mock<IEmailService> _emailService = new();

    private IniciarSesionCommandHandler CrearHandler() =>
        new(_identityService.Object, _jwtTokenService.Object, _refreshTokenService.Object, _emailService.Object);

    [Fact]
    public async Task Handle_CredencialesValidas_DevuelveTokens()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _identityService
            .Setup(s => s.ValidarCredencialesAsync("ana", "clave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidarCredencialesResultado(true, false, usuarioId));
        _identityService
            .Setup(s => s.RequiereMfaAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
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
        resultado.RequiereMfa.Should().BeFalse();
        resultado.AccessToken.Should().Be("token-jwt");
        resultado.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_CuentaConMfaHabilitado_EnviaCodigoYNoDevuelveTokens()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _identityService
            .Setup(s => s.ValidarCredencialesAsync("oficial", "clave", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidarCredencialesResultado(true, false, usuarioId));
        _identityService
            .Setup(s => s.RequiereMfaAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _identityService
            .Setup(s => s.GenerarCodigoMfaAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("123456");
        _identityService
            .Setup(s => s.ObtenerUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioBasico(usuarioId, "oficial", "oficial@sitram.local"));
        var handler = CrearHandler();

        // Act
        var resultado = await handler.Handle(new IniciarSesionCommand("oficial", "clave"), CancellationToken.None);

        // Assert
        resultado.RequiereMfa.Should().BeTrue();
        resultado.UsuarioId.Should().Be(usuarioId);
        resultado.AccessToken.Should().BeNull();
        _emailService.Verify(e => e.EnviarAsync(
            "oficial@sitram.local", It.IsAny<string>(), It.Is<string>(c => c.Contains("123456")), It.IsAny<CancellationToken>()),
            Times.Once);
        _jwtTokenService.Verify(
            s => s.GenerarAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
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
