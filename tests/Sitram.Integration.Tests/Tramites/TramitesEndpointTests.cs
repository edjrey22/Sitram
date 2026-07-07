using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests.Tramites;

[Collection("Integración")]
public class TramitesEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task Post_IniciarTramite_Devuelve201_YPersisteEnBd()
    {
        // Arrange
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var body = new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9001" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tramites", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
        var existe = await db.Tramites.AnyAsync(t => t.Codigo == "TRA-2026-9001");
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task Post_IniciarTramite_SinCodigo_Devuelve400()
    {
        // Arrange
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var body = new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tramites", body);

        // Assert (el validador rechaza el código vacío -> Problem Details 400)
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_IniciarTramite_SinToken_Devuelve401()
    {
        // Arrange: cliente SIN autenticar
        var client = factory.CreateClient();
        var body = new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9005" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tramites", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_IniciarTramite_ConRolSinPermiso_Devuelve403()
    {
        // Arrange: un Auditor no tiene el permiso "tramite:iniciar"
        var client = await factory.CrearClienteAutenticadoAsync("Auditor");
        var body = new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9006" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tramites", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_TramiteExistente_Devuelve200_ConEstadoBorrador()
    {
        // Arrange: crear un trámite y recuperar su id
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var creacion = await client.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9010" });
        var creado = await creacion.Content.ReadFromJsonAsync<CreadoResponse>();

        // Act
        var response = await client.GetAsync($"/api/tramites/{creado!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<EstadoResponse>();
        dto!.Codigo.Should().Be("TRA-2026-9010");
        dto.Estado.Should().Be("Borrador");
    }

    [Fact]
    public async Task Get_TramiteInexistente_Devuelve404()
    {
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await client.GetAsync($"/api/tramites/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Flujo_CrearEnviarRevisarAprobar_TerminaEnAprobado()
    {
        // Arrange: cada rol del flujo real usa su propio usuario autenticado
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var mesaDePartes = await factory.CrearClienteAutenticadoAsync("MesaDePartes");
        var jefeDeArea = await factory.CrearClienteAutenticadoAsync("JefeDeArea");
        var id = await CrearTramite(ciudadano, "TRA-2026-9100");

        // Act: recorrido válido de la máquina de estados por HTTP, cada paso con su rol
        (await ciudadano.PostAsync($"/api/tramites/{id}/enviar", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await mesaDePartes.PostAsync($"/api/tramites/{id}/revision", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await jefeDeArea.PostAsync($"/api/tramites/{id}/aprobar", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert
        var estado = await ciudadano.GetFromJsonAsync<EstadoResponse>($"/api/tramites/{id}");
        estado!.Estado.Should().Be("Aprobado");
    }

    [Fact]
    public async Task Aprobar_TramiteEnBorrador_Devuelve409()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var jefeDeArea = await factory.CrearClienteAutenticadoAsync("JefeDeArea");
        var id = await CrearTramite(ciudadano, "TRA-2026-9101");

        // Act: transición inválida (Borrador -> Aprobado), con el permiso correcto
        var response = await jefeDeArea.PostAsync($"/api/tramites/{id}/aprobar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Enviar_TramiteInexistente_Devuelve404()
    {
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await client.PostAsync($"/api/tramites/{Guid.NewGuid()}/enviar", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Guid> CrearTramite(HttpClient client, string codigo)
    {
        var respuesta = await client.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo });
        var creado = await respuesta.Content.ReadFromJsonAsync<CreadoResponse>();
        return creado!.Id;
    }

    private sealed record CreadoResponse(Guid Id);
    private sealed record EstadoResponse(Guid Id, string Codigo, string Estado, DateTime CreadoUtc);
}
