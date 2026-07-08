using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sitram.Integration.Tests.Auth;

[Collection("Integración")]
public class AuthEndpointTests(SitramWebFactory factory)
{
    private static int _contadorDni;

    // Rango distinto al de AuthTestHelper (10_000_000+) para evitar colisiones de Dni único.
    private static string GenerarDni() => (20_000_000 + Interlocked.Increment(ref _contadorDni)).ToString();

    private static object PerfilDePrueba(string userName, string email, string password) => new
    {
        userName, email, password,
        nombres = "Nombre", apellidos = "Apellido", dni = GenerarDni(), telefono = "987654321", direccion = "Av. Test 123",
    };

    [Fact]
    public async Task Registro_ConDatosValidos_Devuelve201()
    {
        var client = factory.CreateClient();
        var userName = $"reg_{Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/auth/registro",
            PerfilDePrueba(userName, $"{userName}@test.local", "Clave#Segura123"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_ConCredencialesValidas_DevuelveAccessTokenYRefreshToken()
    {
        // Arrange
        var client = factory.CreateClient();
        var userName = $"login_{Guid.NewGuid():N}";
        const string password = "Clave#Segura123";
        await client.PostAsJsonAsync("/api/auth/registro", PerfilDePrueba(userName, $"{userName}@test.local", password));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName, password });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = await response.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>();
        tokens!.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ConContrasenaIncorrecta_Devuelve401()
    {
        // Arrange
        var client = factory.CreateClient();
        var userName = $"malapass_{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/auth/registro",
            PerfilDePrueba(userName, $"{userName}@test.local", "Clave#Segura123"));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName, password = "Incorrecta1!" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_TrasCincoIntentosFallidos_BloqueaLaCuenta()
    {
        // Arrange (RF-003): 5 intentos fallidos consecutivos bloquean la cuenta
        var client = factory.CreateClient();
        var userName = $"bloqueo_{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/auth/registro",
            PerfilDePrueba(userName, $"{userName}@test.local", "Clave#Segura123"));

        // Act: 5 intentos con contraseña incorrecta
        for (var i = 0; i < 5; i++)
            await client.PostAsJsonAsync("/api/auth/login", new { userName, password = "Incorrecta1!" });

        // el sexto intento, incluso con la contraseña correcta, debe rechazarse por bloqueo
        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName, password = "Clave#Segura123" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefrescarToken_ConTokenValido_EmiteNuevoParYRevocaElAnterior()
    {
        // Arrange
        var client = factory.CreateClient();
        var userName = $"refresh_{Guid.NewGuid():N}";
        const string password = "Clave#Segura123";
        await client.PostAsJsonAsync("/api/auth/registro", PerfilDePrueba(userName, $"{userName}@test.local", password));
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName, password });
        var tokensOriginales = (await login.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>())!;

        // Act: primera renovación (debe funcionar)
        var refresco = await client.PostAsJsonAsync("/api/auth/refrescar",
            new { refreshToken = tokensOriginales.RefreshToken });

        // Assert
        refresco.StatusCode.Should().Be(HttpStatusCode.OK);
        var nuevosTokens = await refresco.Content.ReadFromJsonAsync<AuthTestHelper.TokenResponseTest>();
        nuevosTokens!.RefreshToken.Should().NotBe(tokensOriginales.RefreshToken);

        // Act 2: reutilizar el refresh token original (ya revocado por la rotación) debe fallar
        var reintento = await client.PostAsJsonAsync("/api/auth/refrescar",
            new { refreshToken = tokensOriginales.RefreshToken });

        // Assert 2
        reintento.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
