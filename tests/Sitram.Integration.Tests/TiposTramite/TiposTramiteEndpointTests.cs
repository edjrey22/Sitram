using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.TiposTramite;

[Collection("Integración")]
public class TiposTramiteEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task Catalogo_SinAutenticacion_Devuelve200_ConElTipoDePrueba()
    {
        // Arrange: catálogo público (RF-014), no requiere token
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/tipostramite");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var catalogo = await response.Content.ReadFromJsonAsync<List<TipoResumen>>();
        catalogo.Should().Contain(t => t.Nombre == "Licencia de funcionamiento");
    }

    [Fact]
    public async Task Crear_ComoAdministrador_Devuelve201_YApareceEnElCatalogo()
    {
        // Arrange
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");

        // Act
        var creacion = await admin.PostAsJsonAsync("/api/tipostramite",
            new { nombre = "Certificado de zonificación", descripcion = "Uso de suelo", areaResponsable = "Desarrollo Urbano", tasa = 80m });

        // Assert
        creacion.StatusCode.Should().Be(HttpStatusCode.Created);
        var catalogo = await factory.CreateClient().GetFromJsonAsync<List<TipoResumen>>("/api/tipostramite");
        catalogo.Should().Contain(t => t.Nombre == "Certificado de zonificación");
    }

    [Fact]
    public async Task Crear_ComoCiudadanoSinPermiso_Devuelve403()
    {
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await ciudadano.PostAsJsonAsync("/api/tipostramite",
            new { nombre = "X", descripcion = "Y", areaResponsable = "Z", tasa = 10m });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Desactivar_ExcluyeDelCatalogo_PeroNoLoElimina()
    {
        // Arrange
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var creacion = await admin.PostAsJsonAsync("/api/tipostramite",
            new { nombre = "Trámite temporal", descripcion = "D", areaResponsable = "Rentas", tasa = 20m });
        var creado = (await creacion.Content.ReadFromJsonAsync<IdResponse>())!;

        // Act
        var desactivacion = await admin.PostAsync($"/api/tipostramite/{creado.Id}/desactivar", null);

        // Assert
        desactivacion.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var catalogo = await factory.CreateClient().GetFromJsonAsync<List<TipoResumen>>("/api/tipostramite");
        catalogo.Should().NotContain(t => t.Nombre == "Trámite temporal");

        var detalle = await factory.CreateClient().GetFromJsonAsync<DetalleResponse>($"/api/tipostramite/{creado.Id}");
        detalle!.Activo.Should().BeFalse(); // sigue existiendo, solo desactivado
    }

    [Fact]
    public async Task IniciarTramite_ConTipoTramiteInexistente_Devuelve400()
    {
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 999999, codigo = "TRA-2026-9400" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IniciarTramite_ConTipoTramiteDesactivado_Devuelve400()
    {
        // Arrange
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var creacion = await admin.PostAsJsonAsync("/api/tipostramite",
            new { nombre = "Trámite inactivo", descripcion = "D", areaResponsable = "Rentas", tasa = 20m });
        var creado = (await creacion.Content.ReadFromJsonAsync<IdResponse>())!;
        await admin.PostAsync($"/api/tipostramite/{creado.Id}/desactivar", null);

        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        // Act
        var response = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = creado.Id, codigo = "TRA-2026-9401" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record IdResponse(int Id);
    private sealed record TipoResumen(int Id, string Nombre, string Descripcion, string AreaResponsable, decimal Tasa);
    private sealed record DetalleResponse(int Id, string Nombre, bool Activo);
}
