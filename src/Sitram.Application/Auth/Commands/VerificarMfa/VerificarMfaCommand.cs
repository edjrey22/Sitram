using MediatR;
using Sitram.Application.Auth.Commands.IniciarSesion;

namespace Sitram.Application.Auth.Commands.VerificarMfa;

/// <summary>Completa el segundo paso del login cuando la cuenta exige MFA (RF-005).</summary>
public sealed record VerificarMfaCommand(Guid UsuarioId, string Codigo) : IRequest<TokenResponse>;
