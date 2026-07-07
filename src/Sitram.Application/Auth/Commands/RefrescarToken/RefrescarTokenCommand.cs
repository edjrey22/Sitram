using MediatR;
using Sitram.Application.Auth.Commands.IniciarSesion;

namespace Sitram.Application.Auth.Commands.RefrescarToken;

/// <summary>Rota el refresh token y emite un nuevo par de tokens (RNF-008).</summary>
public sealed record RefrescarTokenCommand(string RefreshToken) : IRequest<TokenResponse>;
