using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Sitram.Api.Middlewares;
using Sitram.Api.Services;
using Sitram.Application;
using Sitram.Application.Common.Interfaces;
using Sitram.Infrastructure;
using Sitram.Infrastructure.Identity;
using Sitram.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Carga explícita de User Secrets por su UserSecretsId: WebApplication.CreateBuilder solo los
// añade automáticamente si detecta ASPNETCORE_ENVIRONMENT=Development, y algunos lanzadores
// (p. ej. depurar desde Visual Studio) no siempre propagan esa variable al proceso. Forzarlo aquí
// garantiza que la cadena real de Supabase y la 'Cifrado:Clave' se lean sin depender del entorno.
// La variante por GUID es opcional por naturaleza (si el archivo de secrets no existe, la fuente
// queda vacía), así que es inofensiva en producción, donde la config viene de variables de entorno.
builder.Configuration.AddUserSecrets("40aa4e79-a870-44c3-aff7-5ca151e5a931"); // UserSecretsId de Sitram.Api

// Logging estructurado (Serilog) — la política de enmascarado de datos personales se añade en Sprint 5 (ADR-0004)
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console());

// Capas de la aplicación (regla de dependencia: Api -> Application/Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// API
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHostedService<AlertaVencimientoBackgroundService>(); // RF-053

// Autenticación JWT (ADR-0005). La clave firmante viene de configuración (User Secrets en
// desarrollo, variables de entorno en producción) — nunca hardcodeada (errores-conocidos 3.4).
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Sin esto, "sub" se remapea a una URI larga de XML-SOAP y ICurrentUserService no lo
        // encuentra: los claims del token quedan exactamente como se emitieron (ADR-0005).
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

// Autorización RBAC por políticas de permiso, no por nombre de rol (ADR-0005).
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("TramiteIniciar", p => p.RequireClaim("permiso", "tramite:iniciar"))
    .AddPolicy("TramiteEnviar", p => p.RequireClaim("permiso", "tramite:enviar"))
    .AddPolicy("TramiteSubsanar", p => p.RequireClaim("permiso", "tramite:subsanar"))
    .AddPolicy("TramiteConsultar", p => p.RequireClaim("permiso", "tramite:consultar"))
    .AddPolicy("TramiteRecepcionar", p => p.RequireClaim("permiso", "tramite:recepcionar"))
    .AddPolicy("TramiteEvaluar", p => p.RequireClaim("permiso", "tramite:evaluar"))
    .AddPolicy("TramiteObservar", p => p.RequireClaim("permiso", "tramite:observar"))
    .AddPolicy("TramiteAprobar", p => p.RequireClaim("permiso", "tramite:aprobar"))
    .AddPolicy("TramiteRechazar", p => p.RequireClaim("permiso", "tramite:rechazar"))
    .AddPolicy("AuditoriaLeer", p => p.RequireClaim("permiso", "auditoria:leer"))
    .AddPolicy("AdministracionGestionar", p => p.RequireClaim("permiso", "administracion:gestionar"))
    .AddPolicy("TramitePagar", p => p.RequireClaim("permiso", "tramite:pagar"))
    .AddPolicy("TramiteAdjuntar", p => p.RequireClaim("permiso", "tramite:adjuntar"))
    .AddPolicy("ReportesLeer", p => p.RequireClaim("permiso", "reportes:leer"))
    .AddPolicy("DatosArco", p => p.RequireClaim("permiso", "datos:arco"));

var app = builder.Build();

// Middleware global: traduce excepciones a Problem Details sin filtrar detalles internos (RNF-006)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsEnvironment("Testing"))
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Semilla idempotente de roles y permisos (modelo-datos.md §6)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
    await IdentitySeeder.SeedAsync(db);
}

app.Run();

/// <summary>Punto de entrada expuesto para las pruebas de integración (WebApplicationFactory).</summary>
public partial class Program { }
