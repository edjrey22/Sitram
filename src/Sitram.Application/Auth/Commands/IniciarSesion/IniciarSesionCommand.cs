using MediatR;

namespace Sitram.Application.Auth.Commands.IniciarSesion;

/// <summary>
/// Inicia sesión (RF-002) con bloqueo tras 5 intentos fallidos (RF-003). Si la cuenta exige
/// segundo factor (RF-005, cuentas de funcionario), no emite tokens todavía: envía el código por
/// correo y el resultado indica que se debe completar <c>VerificarMfaCommand</c>.
/// </summary>
public sealed record IniciarSesionCommand(string UserName, string Password) : IRequest<IniciarSesionResultado>;

/// <summary>
/// Si <see cref="RequiereMfa"/> es verdadero, los campos de token son nulos y <see cref="UsuarioId"/>
/// identifica el desafío pendiente frente a <c>POST /api/auth/login/verificar-mfa</c>.
/// </summary>
public sealed record IniciarSesionResultado(
    bool RequiereMfa, Guid? UsuarioId, string? AccessToken, DateTime? ExpiraUtc, string? RefreshToken);

/// <summary>Par de tokens emitido tras un inicio de sesión, una verificación MFA o una renovación (RNF-008).</summary>
public sealed record TokenResponse(string AccessToken, DateTime ExpiraUtc, string RefreshToken);
