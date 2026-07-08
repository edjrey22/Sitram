using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using FluentAssertions;

namespace Sitram.Integration.Tests.Tramites;

[Collection("Integración")]
public class DocumentosEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task AdjuntarDocumento_ConPdfValido_Devuelve201_YQuedaListable()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var tramiteId = await CrearTramite(ciudadano, "TRA-2026-9500");
        var contenido = "%PDF-1.4 contenido de prueba"u8.ToArray();

        // Act
        var respuesta = await SubirDocumento(ciudadano, tramiteId, "recibo.pdf", contenido);

        // Assert
        respuesta.StatusCode.Should().Be(HttpStatusCode.Created);
        var lista = await ciudadano.GetFromJsonAsync<List<DocumentoDto>>($"/api/tramites/{tramiteId}/documentos");
        lista.Should().ContainSingle(d => d.NombreArchivo == "recibo.pdf");
    }

    [Fact]
    public async Task AdjuntarDocumento_ConExtensionNoPermitida_Devuelve400()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var tramiteId = await CrearTramite(ciudadano, "TRA-2026-9501");

        // Act
        var respuesta = await SubirDocumento(ciudadano, tramiteId, "malicioso.exe", [1, 2, 3]);

        // Assert
        respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DescargarDocumento_DevuelveElMismoContenidoSubido()
    {
        // Arrange
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var tramiteId = await CrearTramite(ciudadano, "TRA-2026-9502");
        var contenidoOriginal = "%PDF-1.4 contenido único de prueba"u8.ToArray();
        var subida = await SubirDocumento(ciudadano, tramiteId, "adjunto.pdf", contenidoOriginal);
        var creado = (await subida.Content.ReadFromJsonAsync<DocumentoCreadoResponse>())!;

        // Act
        var descarga = await ciudadano.GetAsync($"/api/tramites/{tramiteId}/documentos/{creado.DocumentoId}");

        // Assert
        descarga.StatusCode.Should().Be(HttpStatusCode.OK);
        var contenidoDescargado = await descarga.Content.ReadAsByteArrayAsync();
        contenidoDescargado.Should().BeEquivalentTo(contenidoOriginal);

        // Verifica además que el hash SHA-256 calculado coincide con lo esperado (integridad)
        var lista = await ciudadano.GetFromJsonAsync<List<DocumentoDto>>($"/api/tramites/{tramiteId}/documentos");
        var hashEsperado = Convert.ToHexStringLower(SHA256.HashData(contenidoOriginal));
        lista.Should().Contain(d => d.HashSha256 == hashEsperado);
    }

    [Fact]
    public async Task AdjuntarDocumento_ATramiteAjeno_ComoOtroCiudadano_NoImpideLaSubida_PeroRequierePermiso()
    {
        // Arrange: un Auditor no tiene el permiso "tramite:adjuntar"
        var ciudadano = await factory.CrearClienteAutenticadoAsync("Ciudadano");
        var auditor = await factory.CrearClienteAutenticadoAsync("Auditor");
        var tramiteId = await CrearTramite(ciudadano, "TRA-2026-9503");

        // Act
        var respuesta = await SubirDocumento(auditor, tramiteId, "recibo.pdf", [1, 2, 3]);

        // Assert
        respuesta.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static async Task<Guid> CrearTramite(HttpClient client, string codigo)
    {
        var respuesta = await client.PostAsJsonAsync("/api/tramites",
            new { ciudadanoId = Guid.NewGuid(), tipoTramiteId = 1, codigo });
        var creado = (await respuesta.Content.ReadFromJsonAsync<TramiteCreadoResponse>())!;
        return creado.Id;
    }

    private static async Task<HttpResponseMessage> SubirDocumento(HttpClient client, Guid tramiteId, string nombreArchivo, byte[] contenido)
    {
        using var formulario = new MultipartFormDataContent();
        using var archivoContenido = new ByteArrayContent(contenido);
        formulario.Add(archivoContenido, "archivo", nombreArchivo);
        return await client.PostAsync($"/api/tramites/{tramiteId}/documentos", formulario);
    }

    private sealed record TramiteCreadoResponse(Guid Id);
    private sealed record DocumentoCreadoResponse(Guid DocumentoId);
    private sealed record DocumentoDto(Guid Id, string NombreArchivo, string HashSha256, DateTime SubidoUtc);
}
