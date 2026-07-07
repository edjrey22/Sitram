using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Auth.Commands.IniciarSesion;
using Sitram.Application.Auth.Commands.RefrescarToken;
using Sitram.Application.Auth.Commands.Registrar;

namespace Sitram.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Registra un ciudadano (RF-001).</summary>
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new RegistrarCiudadanoCommand(request.UserName, request.Email, request.Password), ct);
        return Created(string.Empty, new { id });
    }

    /// <summary>Inicia sesión y emite un JWT + refresh token (RF-002, RF-003).</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var tokens = await sender.Send(new IniciarSesionCommand(request.UserName, request.Password), ct);
        return Ok(tokens);
    }

    /// <summary>Rota el refresh token y emite un nuevo par de tokens (RNF-008).</summary>
    [HttpPost("refrescar")]
    public async Task<IActionResult> Refrescar([FromBody] RefrescarRequest request, CancellationToken ct)
    {
        var tokens = await sender.Send(new RefrescarTokenCommand(request.RefreshToken), ct);
        return Ok(tokens);
    }
}

public sealed record RegistroRequest(string UserName, string Email, string Password);
public sealed record LoginRequest(string UserName, string Password);
public sealed record RefrescarRequest(string RefreshToken);
