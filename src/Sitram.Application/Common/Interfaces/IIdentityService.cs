namespace Sitram.Application.Common.Interfaces;

/// <summary>
/// Puerto hacia la gestión de identidad (Identity + RBAC). La implementación vive en
/// Infrastructure porque depende del framework de Identity (ADR-0002: Domain/Application
/// no dependen de frameworks).
/// </summary>
public interface IIdentityService
{
    /// <summary>Crea un usuario con contraseña y le asigna el rol "Ciudadano" (RF-001).</summary>
    Task<CrearUsuarioResultado> CrearUsuarioAsync(
        string userName, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida credenciales aplicando el bloqueo tras intentos fallidos (RF-003).
    /// </summary>
    Task<ValidarCredencialesResultado> ValidarCredencialesAsync(
        string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>Códigos de permiso (p. ej. "tramite:aprobar") del usuario, vía sus roles.</summary>
    Task<IReadOnlyList<string>> ObtenerPermisosAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<UsuarioBasico?> ObtenerUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}

public sealed record CrearUsuarioResultado(bool Succeeded, Guid UsuarioId, IReadOnlyList<string> Errores);

public sealed record ValidarCredencialesResultado(bool Succeeded, bool BloqueadoTemporalmente, Guid UsuarioId);

public sealed record UsuarioBasico(Guid Id, string UserName);
