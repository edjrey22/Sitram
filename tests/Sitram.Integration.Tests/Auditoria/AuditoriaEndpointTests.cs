using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.Auditoria;

[Collection("Integración")]
public class AuditoriaEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task ObtenerAuditoria_TrasCrearYEnviar_RegistraAmbasAcciones()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var auditor = await factory.CrearClienteAutenticadoAsync("Auditor");

        var creacion = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9300" });
        var creado = (await creacion.Content.ReadFromJsonAsync<CreadoResponse>())!;
        await ciudadano.PostAsync($"/api/tramites/{creado.Id}/enviar", null);

        // Act
        var response = await auditor.GetAsync($"/api/tramites/{creado.Id}/auditoria");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventos = await response.Content.ReadFromJsonAsync<List<EventoDto>>();
        eventos.Should().HaveCount(2);
        eventos![0].Accion.Should().Be("TramiteCreado");
        eventos[1].Accion.Should().Be("Borrador->Recibido");
    }

    [Fact]
    public async Task ObtenerAuditoria_ComoCiudadanoSinPermiso_Devuelve403()
    {
        // Arrange: un Ciudadano no tiene el permiso "auditoria:leer"
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var creacion = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo = "TRA-2026-9301" });
        var creado = (await creacion.Content.ReadFromJsonAsync<CreadoResponse>())!;

        // Act
        var response = await ciudadano.GetAsync($"/api/tramites/{creado.Id}/auditoria");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private sealed record CreadoResponse(Guid Id);
    private sealed record EventoDto(long EventoId, string Accion, string? DatosAntes, string? DatosDespues, DateTime FechaUtc);
}
