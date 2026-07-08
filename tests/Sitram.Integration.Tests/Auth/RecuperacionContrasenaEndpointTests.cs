using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Sitram.Integration.Tests.Auth;

[Collection("Integración")]
public class RecuperacionContrasenaEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task FlujoCompleto_RecuperarYRestablecer_PermiteIniciarSesionConLaNuevaContrasena()
    {
        // Arrange: registrar un usuario
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var client = factory.CreateClient();
        var userName = $"recuperar_{Guid.NewGuid():N}";
        var correo = $"{userName}@test.local";
        const string contrasenaOriginal = "Clave#Segura123";
        const string contrasenaNueva = "NuevaClave#456";

        var registro = await client.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email = correo, password = contrasenaOriginal,
            nombres = "Nombre", apellidos = "Apellido", dni = "60000001", telefono = "987654321", direccion = "Av. Test 123",
        });
        var registrado = (await registro.Content.ReadFromJsonAsync<IdResponse>())!;

        // Act 1: solicitar recuperación
        var solicitud = await client.PostAsJsonAsync("/api/auth/recuperar-contrasena", new { email = correo });
        solicitud.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var correoEnviado = fakeEmail.Enviados.Last(e => e.Destino == correo && e.Asunto.Contains("Recupera"));
        var token = Uri.UnescapeDataString(Regex.Match(correoEnviado.Cuerpo, @"token=([^\s&]+)").Groups[1].Value);

        // Act 2: restablecer con el token recibido
        var restablecimiento = await client.PostAsJsonAsync("/api/auth/restablecer-contrasena",
            new { usuarioId = registrado.Id, token, nuevaContrasena = contrasenaNueva });
        restablecimiento.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert: la contraseña nueva funciona y la vieja ya no
        var loginNueva = await client.PostAsJsonAsync("/api/auth/login", new { userName, password = contrasenaNueva });
        loginNueva.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginVieja = await client.PostAsJsonAsync("/api/auth/login", new { userName, password = contrasenaOriginal });
        loginVieja.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RecuperarContrasena_ConCorreoInexistente_Devuelve204_SinRevelarNada()
    {
        // Arrange (evita enumeración de usuarios): el comportamiento es idéntico exista o no el correo
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/recuperar-contrasena", new { email = "nadie@correo.com" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RestablecerContrasena_ConTokenInvalido_Devuelve401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/restablecer-contrasena",
            new { usuarioId = Guid.NewGuid(), token = "token-invalido", nuevaContrasena = "Clave#Nueva123" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private sealed record IdResponse(Guid Id);
}
