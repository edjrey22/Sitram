using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sitram.Integration.Tests.Seguridad;

[Collection("Integración")]
public class IncidentesSeguridadEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task DesignarOficialDatos_ComoAdministrador_AsignaElRol()
    {
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");

        var response = await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DesignarOficialDatos_ComoCiudadano_Devuelve403()
    {
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await ciudadano.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegistrarIncidente_ConOficialDesignado_NotificaPorCorreoYQuedaVisibleParaElOficial()
    {
        // Arrange: designar al oficial de datos
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });

        // Volver a autenticar: la asignación de rol no se refleja en el token ya emitido.
        var (oficialConRol, _) = await ReautenticarComoOficialAsync(oficialId);

        var fakeEmail = factory.Services.GetRequiredService<FakeEmailService>();

        // Act: se detecta y registra el incidente (RF-065)
        var registro = await admin.PostAsJsonAsync("/api/incidentes-seguridad", new
        {
            titulo = "Acceso no autorizado a la BD",
            descripcion = "Se detectaron múltiples intentos de acceso fallidos desde una IP desconocida.",
            gravedad = "Alta",
        });

        // Assert
        registro.StatusCode.Should().Be(HttpStatusCode.Created);
        var creado = (await registro.Content.ReadFromJsonAsync<IncidenteCreadoResponse>())!;
        creado.Estado.Should().Be("Notificado");
        creado.OficialNotificado.Should().BeTrue();

        fakeEmail.Enviados.Should().Contain(e => e.Asunto.Contains("Incidente de seguridad"));

        var listado = await oficialConRol.GetFromJsonAsync<List<IncidenteDto>>("/api/incidentes-seguridad");
        listado.Should().Contain(i => i.Id == creado.IncidenteId);
    }

    [Fact]
    public async Task RegistrarIncidente_ComoCiudadano_Devuelve403()
    {
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await ciudadano.PostAsJsonAsync("/api/incidentes-seguridad", new
        {
            titulo = "Incidente", descripcion = "Detalle", gravedad = "Baja",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ResolverIncidente_DesdeElOficial_LoCierra()
    {
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var (_, oficialId) = await factory.CrearClienteAutenticadoConIdAsync("Ciudadano");
        await admin.PostAsJsonAsync("/api/proteccion-datos/oficial-datos", new { usuarioId = oficialId });
        var (oficialConRol, _) = await ReautenticarComoOficialAsync(oficialId);

        var registro = await admin.PostAsJsonAsync("/api/incidentes-seguridad", new
        {
            titulo = "Fuga de datos", descripcion = "Detalle del incidente", gravedad = "Critica",
        });
        var creado = (await registro.Content.ReadFromJsonAsync<IncidenteCreadoResponse>())!;

        var resolucion = await oficialConRol.PostAsJsonAsync(
            $"/api/incidentes-seguridad/{creado.IncidenteId}/resolver", new { resolucion = "Se revocaron las credenciales expuestas." });

        resolucion.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var detalle = await oficialConRol.GetFromJsonAsync<IncidenteDto>($"/api/incidentes-seguridad/{creado.IncidenteId}");
        detalle!.Estado.Should().Be("Resuelto");
    }

    /// <summary>
    /// El JWT emitido al registrar al usuario no lleva el permiso "datos:arco" porque ese rol se
    /// asignó después: se vuelve a autenticar para obtener un token con los permisos vigentes.
    /// </summary>
    private async Task<(HttpClient Client, Guid Id)> ReautenticarComoOficialAsync(Guid oficialId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sitram.Infrastructure.Persistence.SitramDbContext>();
        var userName = (await db.Users.SingleAsync(u => u.Id == oficialId)).UserName!;

        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName, password = "Clave#Segura123" });
        login.EnsureSuccessStatusCode();
        var tokens = (await login.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>())!;
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        return (client, oficialId);
    }

    private sealed record IncidenteCreadoResponse(Guid IncidenteId, string Estado, bool OficialNotificado);

    private sealed record IncidenteDto(
        Guid Id, string Titulo, string Descripcion, string Gravedad, string Estado,
        DateTime FechaDeteccionUtc, DateTime? FechaNotificacionUtc, Guid? OficialNotificadoId,
        string? Resolucion, DateTime? FechaResolucionUtc);
}
