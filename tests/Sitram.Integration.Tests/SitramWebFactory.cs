using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Integration.Tests;

/// <summary>
/// Arranca la API en memoria apuntando a una base de datos de prueba en LocalDB
/// (SitramDb_Test), con el esquema recreado desde las migraciones.
/// </summary>
public sealed class SitramWebFactory : WebApplicationFactory<Program>
{
    private const string TestConnection =
        @"Server=(localdb)\MSSQLLocalDB;Database=SitramDb_Test;Trusted_Connection=True;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Clave JWT exclusiva de la ejecución de pruebas (no es un secreto de producción;
        // el entorno "Testing" no carga User Secrets, así que se provee aquí explícitamente).
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "clave-de-pruebas-solo-para-integration-tests-nunca-en-produccion-0123456789",
                ["Jwt:Issuer"] = "Sitram.Api.Tests",
                ["Jwt:Audience"] = "Sitram.Clientes.Tests",
                ["Jwt:MinutosExpiracion"] = "15",
                ["Jwt:DiasExpiracionRefresh"] = "7",
            });
        });

        builder.ConfigureServices(services =>
        {
            // Sustituir el DbContext de producción por el de la BD de prueba
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SitramDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<SitramDbContext>(o => o.UseSqlServer(TestConnection));

            // Recrear el esquema de la BD de prueba desde las migraciones
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();
        });
    }
}
