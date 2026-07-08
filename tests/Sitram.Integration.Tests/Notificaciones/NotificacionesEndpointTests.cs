using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Sitram.Integration.Tests.Notificaciones;

[Collection("Integración")]
public class NotificacionesEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task Registro_EnviaCorreoDeVerificacion()
    {
        // Arrange
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var client = factory.CreateClient();
        var userName = $"notif_{Guid.NewGuid():N}";
        var correo = $"{userName}@test.local";

        // Act
        await client.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email = correo, password = "Clave#Segura123",
            nombres = "Nombre", apellidos = "Apellido", dni = "40000001", telefono = "987654321", direccion = "Av. Test 123",
        });

        // Assert
        fakeEmail.Enviados.Should().Contain(e => e.Destino == correo && e.Asunto.Contains("Verifica tu correo"));
    }

    [Fact]
    public async Task ConfirmarEmail_ConTokenDelCorreoEnviado_Devuelve204()
    {
        // Arrange
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var client = factory.CreateClient();
        var userName = $"confirmar_{Guid.NewGuid():N}";
        var correo = $"{userName}@test.local";

        var registro = await client.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email = correo, password = "Clave#Segura123",
            nombres = "Nombre", apellidos = "Apellido", dni = "40000002", telefono = "987654321", direccion = "Av. Test 123",
        });
        var registrado = (await registro.Content.ReadFromJsonAsync<IdResponse>())!;

        // Extraer el token del enlace simulado en el cuerpo del correo
        var correoEnviado = fakeEmail.Enviados.Last(e => e.Destino == correo);
        var token = Uri.UnescapeDataString(Regex.Match(correoEnviado.Cuerpo, @"token=([^\s&]+)").Groups[1].Value);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/confirmar-email", new { usuarioId = registrado.Id, token });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ConfirmarEmail_ConTokenInvalido_Devuelve401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/confirmar-email",
            new { usuarioId = Guid.NewGuid(), token = "token-invalido" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Observar_EnviaCorreoDeNotificacionAlCiudadano()
    {
        // Arrange
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var (ciudadano, revisor, correoCiudadano, codigo) = await PrepararTramiteEnRevision();

        // Act
        await revisor.PostAsJsonAsync($"/api/tramites/{codigo.TramiteId}/observar", new { motivo = "Falta el recibo de pago." });

        // Assert
        fakeEmail.Enviados.Should().Contain(e =>
            e.Destino == correoCiudadano && e.Asunto.Contains(codigo.Codigo) && e.Asunto.Contains("subsanación"));
    }

    [Fact]
    public async Task Aprobar_EnviaCorreoDeNotificacionAlCiudadano()
    {
        // Arrange
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var (ciudadano, _, correoCiudadano, codigo) = await PrepararTramiteEnRevision();
        var jefeDeArea = await factory.CrearClienteAutenticadoAsync("JefeDeArea");

        // Act
        await jefeDeArea.PostAsync($"/api/tramites/{codigo.TramiteId}/aprobar", null);

        // Assert
        fakeEmail.Enviados.Should().Contain(e =>
            e.Destino == correoCiudadano && e.Asunto.Contains(codigo.Codigo) && e.Asunto.Contains("aprobado"));
    }

    [Fact]
    public async Task Rechazar_EnviaCorreoDeNotificacionAlCiudadano()
    {
        // Arrange
        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();
        var (ciudadano, _, correoCiudadano, codigo) = await PrepararTramiteEnRevision();
        var jefeDeArea = await factory.CrearClienteAutenticadoAsync("JefeDeArea");

        // Act
        await jefeDeArea.PostAsJsonAsync($"/api/tramites/{codigo.TramiteId}/rechazar", new { motivo = "No cumple los requisitos." });

        // Assert
        fakeEmail.Enviados.Should().Contain(e =>
            e.Destino == correoCiudadano && e.Asunto.Contains(codigo.Codigo) && e.Asunto.Contains("rechazado"));
    }

    private async Task<(HttpClient Ciudadano, HttpClient Revisor, string CorreoCiudadano, (Guid TramiteId, string Codigo) Codigo)>
        PrepararTramiteEnRevision()
    {
        var userName = $"tramitador_{Guid.NewGuid():N}";
        var correo = $"{userName}@test.local";
        var ciudadano = factory.CreateClient();
        var dni = (50_000_000 + Random.Shared.Next(1_000_000)).ToString();

        var registro = await ciudadano.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email = correo, password = "Clave#Segura123",
            nombres = "Nombre", apellidos = "Apellido", dni, telefono = "987654321", direccion = "Av. Test 123",
        });
        var registrado = (await registro.Content.ReadFromJsonAsync<IdResponse>())!;
        var login = await ciudadano.PostAsJsonAsync("/api/auth/login", new { userName, password = "Clave#Segura123" });
        var tokens = (await login.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>())!;
        ciudadano.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var revisor = await factory.CrearClienteAutenticadoAsync("Revisor");
        var mesaDePartes = await factory.CrearClienteAutenticadoAsync("MesaDePartes");

        var codigo = $"TRA-2026-{Guid.NewGuid():N}"[..20];
        var creacion = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = registrado.Id, tipoTramiteId = 1, codigo });
        var tramite = (await creacion.Content.ReadFromJsonAsync<TramiteCreadoResponse>())!;

        await ciudadano.PostAsync($"/api/tramites/{tramite.Id}/enviar", null);
        await mesaDePartes.PostAsync($"/api/tramites/{tramite.Id}/revision", null);

        return (ciudadano, revisor, correo, (tramite.Id, codigo));
    }

    private sealed record IdResponse(Guid Id);
    private sealed record TramiteCreadoResponse(Guid Id);
}
