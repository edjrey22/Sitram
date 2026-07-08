using MediatR;

namespace Sitram.Application.Auth.Commands.Registrar;

/// <summary>
/// Registra un ciudadano con verificación de correo (RF-001) y crea su perfil de datos
/// personales (comparte Id con el usuario de Identity, relación 1:1 de modelo-datos.md).
/// </summary>
public sealed record RegistrarCiudadanoCommand(
    string UserName, string Email, string Password,
    string Nombres, string Apellidos, string Dni, string Telefono, string Direccion) : IRequest<Guid>;
