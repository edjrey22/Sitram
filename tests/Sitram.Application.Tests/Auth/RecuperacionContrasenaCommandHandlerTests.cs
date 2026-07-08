using FluentAssertions;
using Moq;
using Sitram.Application.Auth.Commands.RestablecerContrasena;
using Sitram.Application.Auth.Commands.SolicitarRecuperacionContrasena;
using Sitram.Application.Common.Exceptions;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Application.Tests.Auth;

public class RecuperacionContrasenaCommandHandlerTests
{
    [Fact]
    public async Task SolicitarRecuperacion_ConCorreoExistente_EnviaElEnlace()
    {
        // Arrange
        var identityService = new Mock<IIdentityService>();
        var emailService = new Mock<IEmailService>();
        var usuario = new UsuarioBasico(Guid.NewGuid(), "ana");
        identityService.Setup(s => s.ObtenerUsuarioPorEmailAsync("ana@correo.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        identityService
            .Setup(s => s.GenerarTokenRecuperacionContrasenaAsync(usuario.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync("token-abc");
        var handler = new SolicitarRecuperacionContrasenaCommandHandler(identityService.Object, emailService.Object);

        // Act
        await handler.Handle(new SolicitarRecuperacionContrasenaCommand("ana@correo.com"), CancellationToken.None);

        // Assert
        emailService.Verify(s => s.EnviarAsync(
            "ana@correo.com", It.Is<string>(a => a.Contains("Recupera")), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SolicitarRecuperacion_ConCorreoInexistente_NoEnviaNadaYNoLanza()
    {
        // Arrange: no revelar si el correo existe (evita enumeración de usuarios)
        var identityService = new Mock<IIdentityService>();
        var emailService = new Mock<IEmailService>();
        identityService
            .Setup(s => s.ObtenerUsuarioPorEmailAsync("nadie@correo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioBasico?)null);
        var handler = new SolicitarRecuperacionContrasenaCommandHandler(identityService.Object, emailService.Object);

        // Act
        var act = async () => await handler.Handle(new SolicitarRecuperacionContrasenaCommand("nadie@correo.com"), CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        emailService.Verify(s => s.EnviarAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RestablecerContrasena_ConTokenInvalido_LanzaAutenticacionInvalidaException()
    {
        // Arrange
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(s => s.RestablecerContrasenaAsync(It.IsAny<Guid>(), "malo", It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var handler = new RestablecerContrasenaCommandHandler(identityService.Object);

        // Act
        var act = async () => await handler.Handle(
            new RestablecerContrasenaCommand(Guid.NewGuid(), "malo", "Nueva#Clave123"), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AutenticacionInvalidaException>();
    }
}
