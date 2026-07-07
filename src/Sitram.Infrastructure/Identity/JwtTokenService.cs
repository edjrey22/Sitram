using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Identity;

/// <summary>
/// Emite JWT firmados con HMAC-SHA256. Cada permiso del usuario se incluye como un claim
/// <c>"permiso"</c> (ADR-0005), que las políticas de autorización de la Api evalúan.
/// </summary>
public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public (string Token, DateTime ExpiraUtc) GenerarAccessToken(Guid usuarioId, string userName, IEnumerable<string> permisos)
    {
        var minutos = configuration.GetValue("Jwt:MinutosExpiracion", 15); // RNF-008: ≤ 15 min
        var expiraUtc = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(permisos.Select(p => new Claim("permiso", p)));

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiraUtc,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraUtc);
    }
}
