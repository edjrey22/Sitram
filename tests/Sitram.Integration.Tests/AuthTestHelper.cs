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
    public static async Task<HttpClient> CrearClienteAutenticadoAsync(this SitramWebFactory factory, string rol = "Ciudadano")
    {
        var client = factory.CreateClient();
        var sufijo = $"{Interlocked.Increment(ref _contador)}_{Guid.NewGuid():N}";
        var userName = $"usuario{sufijo}";
        var email = $"{userName}@test.local";
        const string password = "Clave#Segura123";

        var registro = await client.PostAsJsonAsync("/api/auth/registro", new { userName, email, password });
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
        return client;
    }

    private sealed record IdResponse(Guid Id);
    public sealed record TokenResponseTest(string AccessToken, DateTime ExpiraUtc, string RefreshToken);
}
