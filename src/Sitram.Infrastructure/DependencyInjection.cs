using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;
using Sitram.Infrastructure.Almacenamiento;
using Sitram.Infrastructure.Identity;
using Sitram.Infrastructure.Notificaciones;
using Sitram.Infrastructure.Persistence;
using Sitram.Infrastructure.Persistence.Cifrado;

namespace Sitram.Infrastructure;

/// <summary>Registro de servicios de infraestructura (persistencia, seguridad, servicios externos).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Cifrado de columna (RNF-003): singleton, solo lee la clave una vez de configuración.
        services.AddSingleton<CifradoColumna>();

        // DbContext con ciclo de vida Scoped (una instancia por petición) — errores-conocidos 2.2
        services.AddDbContext<SitramDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SitramDb")));

        services.AddScoped<ITramiteRepository, TramiteRepository>();
        services.AddScoped<ITramitesReadService, TramitesReadService>();
        services.AddScoped<ITipoTramiteRepository, TipoTramiteRepository>();
        services.AddScoped<ITiposTramiteReadService, TiposTramiteReadService>();
        services.AddScoped<ICiudadanoRepository, CiudadanoRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddSingleton<IAlmacenamientoArchivos, AlmacenamientoArchivosLocal>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SitramDbContext>());

        // Identity: solo usuarios (sin sus tablas de rol; el RBAC usa Rol/Permiso propios)
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                // RF-003: bloqueo tras 5 intentos fallidos
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                // RNF-002: complejidad razonable (bcrypt/PBKDF2 lo aplica Identity por defecto)
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
            })
            .AddSignInManager()
            .AddEntityFrameworkStores<SitramDbContext>()
            .AddDefaultTokenProviders(); // requerido para el token de confirmación de correo (RF-001)

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IAuditoriaReadService, AuditoriaReadService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
