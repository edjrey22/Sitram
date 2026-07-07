using MediatR;

namespace Sitram.Application.Auth.Commands.Registrar;

/// <summary>Registra un ciudadano con verificación de correo (RF-001).</summary>
public sealed record RegistrarCiudadanoCommand(string UserName, string Email, string Password)
    : IRequest<Guid>;
