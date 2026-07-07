namespace Sitram.Application.Common.Interfaces;

/// <summary>Datos del usuario autenticado de la petición HTTP en curso (para auditoría, RF-070).</summary>
public interface ICurrentUserService
{
    Guid? UsuarioId { get; }

    string? DireccionIp { get; }
}
