using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.Pagos;

[Collection("Integración")]
public class PagosEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task IniciarRevision_ConTasaImpaga_Devuelve402()
    {
        // Arrange
        var (ciudadano, mesaDePartes, tramiteId) = await PrepararTramiteConTasa();

        // Act: Mesa de Partes intenta admitir el expediente sin que se haya pagado la tasa
        var response = await mesaDePartes.PostAsync($"/api/tramites/{tramiteId}/revision", null);

        // Assert
        ((int)response.StatusCode).Should().Be(402);
    }

    [Fact]
    public async Task RegistrarYConfirmarPago_AvanzaElTramiteAutomaticamente()
    {
        // Arrange
        var (ciudadano, mesaDePartes, tramiteId) = await PrepararTramiteConTasa();

        // Act: registrar el pago (RF-041) y confirmarlo (RF-042, modo prueba)
        var registro = await ciudadano.PostAsJsonAsync("/api/pagos", new { tramiteId });
        registro.StatusCode.Should().Be(HttpStatusCode.Created);
        var pago = (await registro.Content.ReadFromJsonAsync<RegistroPagoResponse>())!;
        pago.Monto.Should().Be(150m);

        var confirmacion = await ciudadano.PostAsync($"/api/pagos/{pago.PagoId}/confirmar", null);

        // Assert (RNF-032): el trámite avanza en la misma operación de confirmación
        confirmacion.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var estado = await ciudadano.GetFromJsonAsync<EstadoResponse>($"/api/tramites/{tramiteId}");
        estado!.Estado.Should().Be("EnRevision");
    }

    [Fact]
    public async Task Comprobante_DePagoConfirmado_SeDescargaComoTextoPlano()
    {
        // Arrange
        var (ciudadano, _, tramiteId) = await PrepararTramiteConTasa();
        var registro = await ciudadano.PostAsJsonAsync("/api/pagos", new { tramiteId });
        var pago = (await registro.Content.ReadFromJsonAsync<RegistroPagoResponse>())!;
        await ciudadano.PostAsync($"/api/pagos/{pago.PagoId}/confirmar", null);

        // Act
        var response = await ciudadano.GetAsync($"/api/pagos/{pago.PagoId}/comprobante");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
        var contenido = await response.Content.ReadAsStringAsync();
        contenido.Should().Contain("Comprobante de pago");
    }

    [Fact]
    public async Task Comprobante_DePagoPendiente_Devuelve400()
    {
        // Arrange
        var (ciudadano, _, tramiteId) = await PrepararTramiteConTasa();
        var registro = await ciudadano.PostAsJsonAsync("/api/pagos", new { tramiteId });
        var pago = (await registro.Content.ReadFromJsonAsync<RegistroPagoResponse>())!;

        // Act: aún no se confirmó el pago
        var response = await ciudadano.GetAsync($"/api/pagos/{pago.PagoId}/comprobante");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<(HttpClient Ciudadano, HttpClient MesaDePartes, Guid TramiteId)> PrepararTramiteConTasa()
    {
        var admin = await factory.CrearClienteAutenticadoAsync("Administrador");
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var mesaDePartes = await factory.CrearClienteAutenticadoAsync("MesaDePartes");

        var tipoCreacion = await admin.PostAsJsonAsync("/api/tipostramite",
            new { nombre = $"Trámite con tasa {Guid.NewGuid():N}", descripcion = "D", areaResponsable = "Rentas", tasa = 150m });
        var tipo = (await tipoCreacion.Content.ReadFromJsonAsync<TipoCreadoResponse>())!;

        var tramiteCreacion = await ciudadano.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = tipo.Id, codigo = $"TRA-2026-{Guid.NewGuid():N}"[..20] });
        var tramite = (await tramiteCreacion.Content.ReadFromJsonAsync<TramiteCreadoResponse>())!;
        await ciudadano.PostAsync($"/api/tramites/{tramite.Id}/enviar", null);

        return (ciudadano, mesaDePartes, tramite.Id);
    }

    private sealed record TipoCreadoResponse(int Id);
    private sealed record TramiteCreadoResponse(Guid Id);
    private sealed record RegistroPagoResponse(Guid PagoId, decimal Monto, string ReferenciaPasarela);
    private sealed record EstadoResponse(Guid Id, string Codigo, string Estado, DateTime CreadoUtc);
}
