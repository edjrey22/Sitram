using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sitram.Application.Auth.Commands.ConfirmarEmail;
using Sitram.Application.Auth.Commands.IniciarSesion;
using Sitram.Application.Auth.Commands.RefrescarToken;
using Sitram.Application.Auth.Commands.Registrar;
using Sitram.Application.Auth.Commands.RestablecerContrasena;
using Sitram.Application.Auth.Commands.SolicitarRecuperacionContrasena;

namespace Sitram.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Registra un ciudadano (RF-001).</summary>
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new RegistrarCiudadanoCommand(
            request.UserName, request.Email, request.Password,
            request.Nombres, request.Apellidos, request.Dni, request.Telefono, request.Direccion), ct);
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

    /// <summary>Confirma el correo a partir del enlace enviado al registrarse (RF-001).</summary>
    [HttpPost("confirmar-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarEmail([FromBody] ConfirmarEmailRequest request, CancellationToken ct)
    {
        await sender.Send(new ConfirmarEmailCommand(request.UsuarioId, request.Token), ct);
        return NoContent();
    }

    /// <summary>
    /// Solicita el enlace de recuperación de contraseña (RF-004). Responde igual exista o no
    /// el correo, para no revelar información a quien intente enumerar usuarios.
    /// </summary>
    [HttpPost("recuperar-contrasena")]
    [AllowAnonymous]
    public async Task<IActionResult> RecuperarContrasena([FromBody] RecuperarContrasenaRequest request, CancellationToken ct)
    {
        await sender.Send(new SolicitarRecuperacionContrasenaCommand(request.Email), ct);
        return NoContent();
    }

    /// <summary>Establece la nueva contraseña con el token recibido por correo (RF-004).</summary>
    [HttpPost("restablecer-contrasena")]
    [AllowAnonymous]
    public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaRequest request, CancellationToken ct)
    {
        await sender.Send(new RestablecerContrasenaCommand(request.UsuarioId, request.Token, request.NuevaContrasena), ct);
        return NoContent();
    }
}

public sealed record RegistroRequest(
    string UserName, string Email, string Password,
    string Nombres, string Apellidos, string Dni, string Telefono, string Direccion);
public sealed record LoginRequest(string UserName, string Password);
public sealed record RefrescarRequest(string RefreshToken);
public sealed record ConfirmarEmailRequest(Guid UsuarioId, string Token);
public sealed record RecuperarContrasenaRequest(string Email);
public sealed record RestablecerContrasenaRequest(Guid UsuarioId, string Token, string NuevaContrasena);
