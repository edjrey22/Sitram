using System.IdentityModel.Tokens.Jwt;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Api.Services;

/// <summary>
/// Implementación de <see cref="ICurrentUserService"/> sobre <see cref="IHttpContextAccessor"/>.
/// Vive en la Api (no en Infrastructure) porque depende del contexto HTTP de ASP.NET Core.
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UsuarioId
    {
        get
        {
            var valor = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(valor, out var id) ? id : null;
        }
    }

    public string? DireccionIp => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
