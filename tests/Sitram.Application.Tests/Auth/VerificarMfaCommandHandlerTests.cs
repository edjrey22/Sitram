using FluentAssertions;
using Moq;
using Sitram.Application.Auth.Commands.VerificarMfa;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Tests.Auth;

public class VerificarMfaCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<IRefreshTokenService> _refreshTokenService = new();

    private VerificarMfaCommandHandler CrearHandler() =>
        new(_identityService.Object, _jwtTokenService.Object, _refreshTokenService.Object);

    [Fact]
    public async Task Handle_CodigoValido_EmiteTokens()
    {
        var usuarioId = Guid.NewGuid();
        _identityService
            .Setup(s => s.VerificarCodigoMfaAsync(usuarioId, "123456", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _identityService
            .Setup(s => s.ObtenerUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UsuarioBasico(usuarioId, "oficial", "oficial@sitram.local"));
        _identityService
            .Setup(s => s.ObtenerPermisosAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(["datos:arco"]);
        _jwtTokenService
            .Setup(s => s.GenerarAccessToken(usuarioId, "oficial", It.IsAny<IEnumerable<string>>()))
            .Returns(("token-jwt", DateTime.UtcNow.AddMinutes(15)));
        _refreshTokenService
            .Setup(s => s.EmitirAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("refresh-token");
        var handler = CrearHandler();

        var resultado = await handler.Handle(new VerificarMfaCommand(usuarioId, "123456"), CancellationToken.None);

        resultado.AccessToken.Should().Be("token-jwt");
        resultado.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_CodigoInvalido_LanzaAutenticacionInvalidaException()
    {
        var usuarioId = Guid.NewGuid();
        _identityService
            .Setup(s => s.VerificarCodigoMfaAsync(usuarioId, "000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var handler = CrearHandler();

        var act = async () => await handler.Handle(new VerificarMfaCommand(usuarioId, "000000"), CancellationToken.None);

        await act.Should().ThrowAsync<AutenticacionInvalidaException>();
    }
}
