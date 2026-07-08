using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests.Auth;

/// <summary>
/// MFA por correo para cuentas de funcionario (RF-005). Hoy la única vía de producción que
/// habilita MFA es la designación del Oficial de Datos Personales (RF-066).
/// </summary>
[Collection("Integración")]
public class MfaEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task Login_ConCuentaConMfaHabilitado_NoDevuelveTokensYEnviaCodigo()
    {
        // Arrange: designar a un usuario como Oficial de Datos habilita MFA (RF-005 + RF-066)
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
        var usuario = await db.Users.SingleAsync(u => u.Id == oficialId);
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();

        var client = factory.CreateClient();

        // Act
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName = usuario.UserName, password = "Clave#Segura123" });

        // Assert
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = (await login.Content.ReadFromJsonAsync<LoginResponseDto>())!;
        resultado.RequiereMfa.Should().BeTrue();
        resultado.AccessToken.Should().BeNull();
        resultado.UsuarioId.Should().Be(oficialId);
        fakeEmail.Enviados.Should().Contain(e => e.Destino == usuario.Email && e.Asunto.Contains("código de verificación"));
    }

    [Fact]
    public async Task VerificarMfa_ConCodigoCorrecto_EmiteTokens()
    {
        // Arrange
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
        var usuario = await db.Users.SingleAsync(u => u.Id == oficialId);

        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName = usuario.UserName, password = "Clave#Segura123" });
        var primerPaso = (await login.Content.ReadFromJsonAsync<LoginResponseDto>())!;

        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var codigo = Regex.Match(fakeEmail.Enviados.Last(e => e.Destino == usuario.Email).Cuerpo, @"\d{6}").Value;

        // Act
        var verificacion = await client.PostAsJsonAsync(
            "/api/auth/login/verificar-mfa", new { usuarioId = primerPaso.UsuarioId, codigo });

        // Assert
        verificacion.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = (await verificacion.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>())!;
        tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task VerificarMfa_ConCodigoIncorrecto_Devuelve401()
    {
        // Arrange
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });

        var client = factory.CreateClient();

        // Act: nunca se pidió el código real, el enviado es inválido
        var verificacion = await client.PostAsJsonAsync(
            "/api/auth/login/verificar-mfa", new { usuarioId = oficialId, codigo = "000000" });

        // Assert
        verificacion.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ConCuentaSinMfa_DevuelveTokensDirectamente()
    {
        var (client, _) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");

        // La cuenta de un Ciudadano recién registrado no exige MFA: CrearClienteAutenticadoConIdAsync
        // ya hizo login con éxito directo (si exigiera MFA, esa llamada habría fallado al leer el token).
        client.DefaultRequestHeaders.Authorization.Should().NotBeNull();
    }

    private sealed record LoginResponseDto(bool RequiereMfa, Guid? UsuarioId, string? AccessToken, DateTime? ExpiraUtc, string? RefreshToken);
}
