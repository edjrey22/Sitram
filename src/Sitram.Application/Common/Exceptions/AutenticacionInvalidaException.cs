namespace Sitram.Application.Common.Exceptions;

/// <summary>Credenciales inválidas, cuenta bloqueada o refresh token no vigente (HTTP 401).</summary>
public sealed class AutenticacionInvalidaException(string message) : Exception(message);
