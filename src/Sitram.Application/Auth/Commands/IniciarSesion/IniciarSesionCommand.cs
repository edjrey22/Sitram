using MediatR;

namespace Sitram.Application.Auth.Commands.IniciarSesion;

/// <summary>Inicia sesión y emite un JWT (RF-002) con bloqueo tras 5 intentos fallidos (RF-003).</summary>
public sealed record IniciarSesionCommand(string UserName, string Password) : IRequest<TokenResponse>;

/// <summary>Par de tokens emitido tras un inicio de sesión o una renovación (RNF-008).</summary>
public sealed record TokenResponse(string AccessToken, DateTime ExpiraUtc, string RefreshToken);
