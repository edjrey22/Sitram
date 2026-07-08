using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Application.Ciudadanos.Queries.ObtenerPerfilCiudadano;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests.Ciudadanos;

[Collection("Integración")]
public class CiudadanosEndpointTests(SitramWebFactory factory)
{
    [Fact]
    public async Task ObtenerPerfil_Propio_Devuelve200_ConDatosDescifrados()
    {
        // Arrange
        var (client, id) = await RegistrarYAutenticar();

        // Act
        var response = await client.GetAsync($"/api/ciudadanos/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var perfil = await response.Content.ReadFromJsonAsync<CiudadanoPerfilDto>();
        perfil!.Dni.Should().MatchRegex(@"^\d{8}$");
        perfil.Correo.Should().Contain("@");
    }

    [Fact]
    public async Task Dni_EnLaBaseDeDatos_NoEsLegibleEnTextoPlano()
    {
        // Arrange (RNF-003): el DNI debe estar cifrado a nivel de columna
        var (_, id) = await RegistrarYAutenticar(dni: "55511122");

        // Act: leer directamente el valor crudo de la columna, sin pasar por el ValueConverter
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
        var dniCrudo = await db.Database
            .SqlQuery<byte[]>($"SELECT Dni AS Value FROM Ciudadanos WHERE Id = {id}")
            .FirstAsync();

        // Assert: los bytes crudos NO contienen el DNI en texto plano
        var textoCrudo = System.Text.Encoding.UTF8.GetString(dniCrudo);
        textoCrudo.Should().NotContain("55511122");
    }

    [Fact]
    public async Task ObtenerPerfil_DeOtroCiudadano_Devuelve403()
    {
        // Arrange
        var (_, idPropio) = await RegistrarYAutenticar();
        var (clienteAjeno, _) = await RegistrarYAutenticar();

        // Act: clienteAjeno intenta leer el perfil de idPropio (no el suyo)
        var response = await clienteAjeno.GetAsync($"/api/ciudadanos/{idPropio}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Rectificar_ActualizaElNombre()
    {
        // Arrange
        var (client, id) = await RegistrarYAutenticar();

        // Act
        var response = await client.PutAsJsonAsync($"/api/ciudadanos/{id}",
            new { nombres = "NombreActualizado", apellidos = (string?)null, correo = (string?)null, telefono = (string?)null, direccion = (string?)null });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var perfil = await client.GetFromJsonAsync<CiudadanoPerfilDto>($"/api/ciudadanos/{id}");
        perfil!.Nombres.Should().Be("NombreActualizado");
    }

    [Fact]
    public async Task Anonimizar_MarcaElPerfilComoAnonimizado_YBloqueaRectificacionPosterior()
    {
        // Arrange
        var (client, id) = await RegistrarYAutenticar();

        // Act
        var anonimizacion = await client.PostAsync($"/api/ciudadanos/{id}/anonimizar", null);

        // Assert
        anonimizacion.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var perfil = await client.GetFromJsonAsync<CiudadanoPerfilDto>($"/api/ciudadanos/{id}");
        perfil!.EstaAnonimizado.Should().BeTrue();

        var rectificacion = await client.PutAsJsonAsync($"/api/ciudadanos/{id}",
            new { nombres = "Otro", apellidos = (string?)null, correo = (string?)null, telefono = (string?)null, direccion = (string?)null });
        rectificacion.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Exportar_DevuelveArchivoJsonConLosDatosDelCiudadano()
    {
        // Arrange
        var (client, id) = await RegistrarYAutenticar();

        // Act
        var response = await client.GetAsync($"/api/ciudadanos/{id}/exportar");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        var perfil = await response.Content.ReadFromJsonAsync<CiudadanoPerfilDto>();
        perfil!.Id.Should().Be(id);
    }

    [Fact]
    public async Task OtorgarYRevocarConsentimiento_QuedaReflejadoEnElPerfil()
    {
        // Arrange
        var (client, id) = await RegistrarYAutenticar();

        // Act
        await client.PostAsJsonAsync($"/api/ciudadanos/{id}/consentimientos", new { finalidad = "notificaciones" });
        await client.PostAsJsonAsync($"/api/ciudadanos/{id}/consentimientos/revocar", new { finalidad = "notificaciones" });

        // Assert
        var perfil = await client.GetFromJsonAsync<CiudadanoPerfilDto>($"/api/ciudadanos/{id}");
        var consentimiento = perfil!.Consentimientos.Single(c => c.Finalidad == "notificaciones");
        consentimiento.Otorgado.Should().BeFalse();
        consentimiento.RevocadoUtc.Should().NotBeNull();
    }

    private static int _contadorDni;

    private async Task<(HttpClient Client, Guid Id)> RegistrarYAutenticar(string? dni = null)
    {
        var client = factory.CreateClient();
        var userName = $"arco_{Guid.NewGuid():N}";
        const string password = "Clave#Segura123";
        dni ??= (30_000_000 + Interlocked.Increment(ref _contadorDni)).ToString();

        var registro = await client.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email = $"{userName}@test.local", password,
            nombres = "Nombre", apellidos = "Apellido", dni, telefono = "987654321", direccion = "Av. Test 123",
        });
        registro.EnsureSuccessStatusCode();
        var registrado = (await registro.Content.ReadFromJsonAsync<IdResponse>())!;

        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName, password });
        var tokens = (await login.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>())!;
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        return (client, registrado.Id);
    }

    private sealed record IdResponse(Guid Id);
}
