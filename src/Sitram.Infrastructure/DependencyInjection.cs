using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Infrastructure;

/// <summary>Registro de servicios de infraestructura (persistencia, seguridad, servicios externos).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext con ciclo de vida Scoped (una instancia por petición) — errores-conocidos 2.2
        services.AddDbContext<SitramDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SitramDb")));

        // TODO(SITRAM): registrar repositorios (puertos del dominio),
        // ASP.NET Core Identity + JWT, y los servicios de correo y pagos (ADR-0004, ADR-0005).

        return services;
    }
}
