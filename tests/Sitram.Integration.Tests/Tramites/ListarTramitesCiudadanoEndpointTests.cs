using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.Tramites;

[Collection("Integración")]
public class ListarTramitesCiudadanoEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task Get_ConCiudadanoConDosTramites_DevuelveSoloLosSuyosOrdenadosPorMasReciente()
    {
        // Arrange: dos trámites de un ciudadano y uno de otro (no debe aparecer en el resultado)
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var ciudadanoId = Guid.NewGuid();
        var primero = await CrearTramite(client, ciudadanoId, "TRA-2026-9200");
        await Task.Delay(10); // asegura CreadoUtc distinto para el orden
        var segundo = await CrearTramite(client, ciudadanoId, "TRA-2026-9201");
        await CrearTramite(client, Guid.NewGuid(), "TRA-2026-9202");

        // Act
        var response = await client.GetAsync($"/api/tramites?ciudadanoId={ciudadanoId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagina = await response.Content.ReadFromJsonAsync<PagedResponse>();
        pagina!.TotalCount.Should().Be(2);
        pagina.Items.Should().HaveCount(2);
        pagina.Items[0].Id.Should().Be(segundo);   // el más reciente primero
        pagina.Items[1].Id.Should().Be(primero);
    }

    [Fact]
    public async Task Get_CiudadanoSinTramites_DevuelvePaginaVacia()
    {
        var client = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await client.GetAsync($"/api/tramites?ciudadanoId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagina = await response.Content.ReadFromJsonAsync<PagedResponse>();
        pagina!.TotalCount.Should().Be(0);
        pagina.Items.Should().BeEmpty();
    }

    private static async Task<Guid> CrearTramite(HttpClient client, Guid ciudadanoId, string codigo)
    {
        var respuesta = await client.PostAsJsonAsync("/api/tramites", new { ciudadanoId, tipoTramiteId = 1, codigo });
        var creado = await respuesta.Content.ReadFromJsonAsync<CreadoResponse>();
        return creado!.Id;
    }

    private sealed record CreadoResponse(Guid Id);
    private sealed record ItemResponse(Guid Id, string Codigo, string Estado, DateTime CreadoUtc);
    private sealed record PagedResponse(List<ItemResponse> Items, int TotalCount, int Page, int PageSize, int TotalPages);
}
