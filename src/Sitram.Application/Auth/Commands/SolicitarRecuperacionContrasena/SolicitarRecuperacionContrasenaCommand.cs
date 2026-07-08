using MediatR;

namespace Sitram.Application.Auth.Commands.SolicitarRecuperacionContrasena;

/// <summary>
/// Solicita el enlace de recuperación de contraseña (RF-004). No revela si el correo existe:
/// siempre se comporta igual desde afuera, para no facilitar la enumeración de usuarios.
/// </summary>
public sealed record SolicitarRecuperacionContrasenaCommand(string Email) : IRequest;
