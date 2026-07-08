using MediatR;

namespace Sitram.Application.Auth.Commands.ConfirmarEmail;

/// <summary>Confirma el correo del usuario a partir del token recibido por email (RF-001).</summary>
public sealed record ConfirmarEmailCommand(Guid UsuarioId, string Token) : IRequest;
