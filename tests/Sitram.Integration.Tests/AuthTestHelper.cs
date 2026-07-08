using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Infrastructure.Identity;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests;

/// <summary>Utilidades para registrar/autenticar usuarios de prueba con un rol específico.</summary>
public static class AuthTestHelper
{
    private static int _contador;

    /// <summary>
    /// Registra un usuario nuevo, le asigna el rol indicado (si no es "Ciudadano", el rol por
    /// defecto) y devuelve un <see cref="HttpClient"/> con el JWT ya puesto en el encabezado.
    /// </summary>
    public static async Task<HttpClient> CrearClienteAutenticadoAsync(this SitramWebFactory factory, string rol = "Ciudadano") =>
        (await factory.CrearClienteAutenticadoConIdAsync(rol)).Client;

    /// <summary>
    /// Igual que <see cref="CrearClienteAutenticadoAsync"/>, pero además devuelve el Id del
    /// usuario/ciudadano registrado (comparten Guid, relación 1:1 de modelo-datos.md) — útil
    /// cuando la prueba necesita crear un trámite a nombre de este ciudadano real.
    /// </summary>
    public static async Task<(HttpClient Client, Guid Id)> CrearClienteAutenticadoConIdAsync(
        this SitramWebFactory factory, string rol = "Ciudadano")
    {
        var client = factory.CreateClient();
        var numero = Interlocked.Increment(ref _contador);
        var sufijo = $"{numero}_{Guid.NewGuid():N}";
        var userName = $"usuario{sufijo}";
        var email = $"{userName}@test.local";
        var dni = (10_000_000 + numero).ToString(); // 8 dígitos únicos por usuario de prueba
        const string password = "Clave#Segura123";

        var registro = await client.PostAsJsonAsync("/api/auth/registro", new
        {
            userName, email, password,
            nombres = "Nombre", apellidos = "Apellido", dni, telefono = "987654321", direccion = "Av. Test 123",
        });
        registro.EnsureSuccessStatusCode();
        var registrado = (await registro.Content.ReadFromJsonAsync<IdResponse>())!;

        if (rol != "Ciudadano")
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
            var rolEntidad = await db.Roles.SingleAsync(r => r.Nombre == rol);
            var asignacionesPrevias = await db.UsuarioRoles.Where(ur => ur.UsuarioId == registrado.Id).ToListAsync();
            db.UsuarioRoles.RemoveRange(asignacionesPrevias);
            db.UsuarioRoles.Add(new UsuarioRol { UsuarioId = registrado.Id, RolId = rolEntidad.RolId });
            await db.SaveChangesAsync();
        }

        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName, password });
        login.EnsureSuccessStatusCode();
        var tokens = (await login.Content.ReadFromJsonAsync<TokenResponseTest>())!;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return (client, registrado.Id);
    }

    private sealed record IdResponse(Guid Id);
    public sealed record TokenResponseTest(string AccessToken, DateTime ExpiraUtc, string RefreshToken);
}
