using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Web.Services;

/// <summary>
/// Implementación de <see cref="ICurrentUserService"/> para Blazor Server. Lee del
/// <see cref="AuthenticationStateProvider"/> en vez de <c>IHttpContextAccessor</c> (que deja de
/// estar disponible una vez que el circuito interactivo toma el control de la conexión). Cuando
/// una página protegida llega a ejecutarse, <c>AuthorizeRouteView</c> ya esperó ese mismo estado
/// para decidir si autorizaba la ruta, así que aquí siempre está resuelto (no bloquea).
/// </summary>
public sealed class CurrentUserService(AuthenticationStateProvider authStateProvider, IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    public Guid? UsuarioId
    {
        get
        {
            var principal = authStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;
            var valor = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(valor, out var id) ? id : null;
        }
    }

    public string? DireccionIp => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
