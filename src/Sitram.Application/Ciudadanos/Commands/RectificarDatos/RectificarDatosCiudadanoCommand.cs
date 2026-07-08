using MediatR;

namespace Sitram.Application.Ciudadanos.Commands.RectificarDatos;

/// <summary>Rectifica los datos personales indicados; los campos nulos no se modifican (RF-061).</summary>
public sealed record RectificarDatosCiudadanoCommand(
    Guid CiudadanoId, string? Nombres, string? Apellidos, string? Correo, string? Telefono, string? Direccion) : IRequest;
