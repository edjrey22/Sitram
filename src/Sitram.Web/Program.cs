using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sitram.Application;
using Sitram.Application.Common.Interfaces;
using Sitram.Infrastructure;
using Sitram.Infrastructure.Identity;
using Sitram.Infrastructure.Persistence;
using Sitram.Web.Components;
using Sitram.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Carga explícita de User Secrets por su UserSecretsId: WebApplication.CreateBuilder solo los
// añade automáticamente si detecta ASPNETCORE_ENVIRONMENT=Development, y algunos lanzadores
// (p. ej. depurar desde Visual Studio) no siempre propagan esa variable al proceso. Forzarlo aquí
// garantiza que la cadena real de Supabase y la 'Cifrado:Clave' se lean sin depender del entorno.
// La variante por GUID es opcional por naturaleza (si el archivo de secrets no existe, la fuente
// queda vacía), así que es inofensiva en producción, donde la config viene de variables de entorno.
builder.Configuration.AddUserSecrets("4c348450-55fc-452e-ad26-252bf35f0747"); // UserSecretsId de Sitram.Web

// Capas de la aplicación (regla de dependencia: Web -> Application/Infrastructure, ADR-0006)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Identidad del usuario dentro del circuito interactivo (ver CurrentUserService)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Sesión autenticada como cookie httpOnly en el servidor: la UI Blazor nunca ve ni guarda un
// JWT (ADR-0006, a diferencia de Sitram.Api que sí usa JWT para integraciones externas). Por
// eso los endpoints de abajo llaman a IIdentityService directamente, no a los commands de Auth
// pensados para el flujo JWT de la Api (que además requieren Jwt:Key, no configurado aquí).
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/acceso-denegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Mismas políticas RBAC por permiso que Sitram.Api (ADR-0005): un solo lugar de verdad sobre
// qué permiso exige cada acción, aplicado igual sin importar la puerta de entrada.
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

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints de autenticación FUERA del circuito interactivo: HttpContext.SignInAsync necesita
// escribir el encabezado Set-Cookie antes de que la respuesta se comprometa, algo que ya no es
// posible una vez que un componente interactivo toma el control de la conexión SignalR.
app.MapPost("/auth/login", async (HttpContext http, IIdentityService identityService) =>
{
    var form = await http.Request.ReadFormAsync();
    var userName = form["userName"].ToString();
    var password = form["password"].ToString();

    // Solo rutas locales ("/algo"): un returnUrl absoluto permitiría un open redirect hacia
    // un sitio de phishing tras un login legítimo.
    var returnUrl = form["returnUrl"].ToString();
    var destino = returnUrl.StartsWith('/') && !returnUrl.StartsWith("//") ? returnUrl : "/";

    var resultado = await identityService.ValidarCredencialesAsync(userName, password);
    if (resultado.BloqueadoTemporalmente)
        return Results.Redirect("/login?error=" + Uri.EscapeDataString("La cuenta está bloqueada temporalmente por intentos fallidos."));
    if (!resultado.Succeeded)
        return Results.Redirect("/login?error=" + Uri.EscapeDataString("Usuario o contraseña incorrectos."));

    if (await identityService.RequiereMfaAsync(resultado.UsuarioId))
    {
        var codigo = await identityService.GenerarCodigoMfaAsync(resultado.UsuarioId);
        var usuario = await identityService.ObtenerUsuarioAsync(resultado.UsuarioId);
        if (!string.IsNullOrWhiteSpace(usuario?.Email))
        {
            var emailService = http.RequestServices.GetRequiredService<IEmailService>();
            await emailService.EnviarAsync(
                usuario.Email, "Tu código de verificación de SITRAM",
                $"Tu código de verificación es: {codigo}. Ingrésalo para completar el inicio de sesión.");
        }
        return Results.Redirect($"/verificar-mfa?usuarioId={resultado.UsuarioId}");
    }

    await FirmarCookieAsync(http, identityService, resultado.UsuarioId, userName);
    return Results.Redirect(destino);
});

app.MapPost("/auth/verificar-mfa", async (HttpContext http, IIdentityService identityService, Guid usuarioId) =>
{
    var form = await http.Request.ReadFormAsync();
    var codigo = form["codigo"].ToString();

    var valido = await identityService.VerificarCodigoMfaAsync(usuarioId, codigo);
    if (!valido)
        return Results.Redirect($"/verificar-mfa?usuarioId={usuarioId}&error=1");

    var usuario = await identityService.ObtenerUsuarioAsync(usuarioId);
    if (usuario is null)
        return Results.Redirect("/login?error=" + Uri.EscapeDataString("Usuario no encontrado."));

    await FirmarCookieAsync(http, identityService, usuarioId, usuario.UserName);
    return Results.Redirect("/");
});

app.MapPost("/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

// Semilla idempotente de roles y permisos (modelo-datos.md §6) — igual que en Sitram.Api.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SitramDbContext>();
    await IdentitySeeder.SeedAsync(db);
}

app.Run();

static async Task FirmarCookieAsync(HttpContext http, IIdentityService identityService, Guid usuarioId, string userName)
{
    var permisos = await identityService.ObtenerPermisosAsync(usuarioId);
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
        new(ClaimTypes.Name, userName),
    };
    claims.AddRange(permisos.Select(p => new Claim("permiso", p)));

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
}

/// <summary>Punto de entrada expuesto para pruebas (patrón usado también por Sitram.Api).</summary>
public partial class Program { }
