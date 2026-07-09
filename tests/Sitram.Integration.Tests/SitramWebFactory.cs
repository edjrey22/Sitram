using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.TiposTramite;
using Sitram.Infrastructure.Persistence;
using System.Linq;

namespace Sitram.Integration.Tests;

/// <summary>
/// Arranca la API en memoria apuntando a una base de datos de prueba en Postgres (Supabase), con
/// el esquema recreado desde las migraciones. La cadena base (host/usuario/contraseña, sin
/// credenciales en el código fuente) vive en User Secrets bajo "ConnectionStrings:SitramDbTestBase";
/// aquí solo se le añade un nombre de base único por corrida para evitar choques entre ejecuciones.
/// </summary>
public sealed class SitramWebFactory : WebApplicationFactory<Program>
{
    private readonly string _testConnection = ConstruirCadenaDePrueba();

    private static string ConstruirCadenaDePrueba()
    {
        var configuracion = new ConfigurationBuilder()
            .AddUserSecrets<SitramWebFactory>()
            .Build();

        var baseConexion = configuracion.GetConnectionString("SitramDbTestBase")
            ?? throw new InvalidOperationException(
                "Falta el User Secret 'ConnectionStrings:SitramDbTestBase' del proyecto de pruebas de integración.");

        return $"{baseConexion};Database=sitram_test_{Guid.NewGuid():N}";
    }

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
                // Clave de cifrado de columna, exclusiva de la ejecución de pruebas (RNF-003).
                ["Cifrado:Clave"] = Convert.ToBase64String(Enumerable.Repeat((byte)7, 64).ToArray()),
            });
        });

        builder.ConfigureServices(services =>
        {
            // Sustituir el DbContext de producción por el de la BD de prueba
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SitramDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<SitramDbContext>(o => o.UseNpgsql(_testConnection));



            // Reemplazar el envío real de correo por un doble de prueba (sin SMTP real).
            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor is not null) services.Remove(emailDescriptor);
            services.AddSingleton<FakeEmailService>();
            services.AddSingleton<IEmailService>(sp => sp.GetRequiredService<FakeEmailService>());

            // Recrear el esquema de la BD de prueba desde las migraciones
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();

            // Tipo de trámite de referencia para las pruebas (será Id=1: primer registro,
            // identity autoincremental). Los tests existentes usan tipoTramiteId=1. Tasa = 0
            // (gratuito) para no activar la puerta de pago (RF-043) en pruebas que no la
            // ejercitan; las pruebas de Pagos crean su propio tipo de trámite con tasa > 0.
            db.TiposTramite.Add(TipoTramite.Crear("Licencia de funcionamiento", "Trámite de prueba", "Rentas", 0m));
            db.SaveChanges();
        });
    }
}
