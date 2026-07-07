using Serilog;
using Sitram.Api.Middlewares;
using Sitram.Application;
using Sitram.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddAuthentication();   // TODO(SITRAM): AddJwtBearer con emisor/clave (ADR-0005, Sprint 1)
builder.Services.AddAuthorization();    // TODO(SITRAM): políticas RBAC por claim de permiso

var app = builder.Build();

// Middleware global: traduce excepciones a Problem Details sin filtrar detalles internos (RNF-006)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>Punto de entrada expuesto para las pruebas de integración (WebApplicationFactory).</summary>
public partial class Program { }
