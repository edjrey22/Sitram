using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.Tramites;

[Collection("Integración")]
public class ReporteTramitesEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task ObtenerReporte_ComoJefeDeArea_Devuelve200_ConConteosCoherentes()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var jefeDeArea = await factory.CrearClienteAutenticadoAsync("JefeDeArea");
        var codigo = $"TRA-2026-{Guid.NewGuid():N}"[..20];
        await ciudadano.PostAsJsonAsync("/api/tramites", new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo });

        // Act
        var response = await jefeDeArea.GetAsync("/api/tramites/reporte");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reporte = await response.Content.ReadFromJsonAsync<ReporteDto>();
        reporte!.TotalTramites.Should().BeGreaterThan(0);
        reporte.PorEstado.Should().Contain(e => e.Estado == "Borrador" && e.Cantidad > 0);
        reporte.PorTipo.Should().Contain(t => t.NombreTipo == "Licencia de funcionamiento");
    }

    [Fact]
    public async Task ObtenerReporte_ComoCiudadanoSinPermiso_Devuelve403()
    {
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");

        var response = await ciudadano.GetAsync("/api/tramites/reporte");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record ConteoPorEstadoDto(string Estado, int Cantidad);
    private sealed record ConteoPorTipoDto(int TipoTramiteId, string NombreTipo, int Cantidad);
    private sealed record ReporteDto(int TotalTramites, List<ConteoPorEstadoDto> PorEstado, List<ConteoPorTipoDto> PorTipo);
}
