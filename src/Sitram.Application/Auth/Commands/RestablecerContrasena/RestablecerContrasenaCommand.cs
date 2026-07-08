using MediatR;

namespace Sitram.Application.Auth.Commands.RestablecerContrasena;

/// <summary>Establece la nueva contraseña a partir del token recibido por correo (RF-004).</summary>
public sealed record RestablecerContrasenaCommand(Guid UsuarioId, string Token, string NuevaContrasena) : IRequest;
