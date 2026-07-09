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

    /// <summary>Genera el token de verificación de correo (RF-001).</summary>
    Task<string> GenerarTokenConfirmacionEmailAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Valida el token y marca el correo como verificado (RF-001).</summary>
    Task<bool> ConfirmarEmailAsync(Guid usuarioId, string token, CancellationToken cancellationToken = default);

    /// <summary>Busca el usuario por correo (RF-004); no revela si existe o no al llamador.</summary>
    Task<UsuarioBasico?> ObtenerUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Genera el token de recuperación de contraseña (RF-004).</summary>
    Task<string> GenerarTokenRecuperacionContrasenaAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Valida el token y establece la nueva contraseña (RF-004).</summary>
    Task<bool> RestablecerContrasenaAsync(
        Guid usuarioId, string token, string nuevaContrasena, CancellationToken cancellationToken = default);

    /// <summary>
    /// Designa a un usuario como el único Oficial de Datos Personales (RF-066): le asigna el rol
    /// "OficialDatosPersonales" y retira ese rol a quien lo tuviera antes, para que el cargo sea
    /// singular en todo momento.
    /// </summary>
    Task DesignarOficialDatosAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Usuario actualmente designado como Oficial de Datos Personales, si existe (RF-065, RF-066).</summary>
    Task<UsuarioBasico?> ObtenerOficialDatosActivoAsync(CancellationToken cancellationToken = default);

    /// <summary>Marca la cuenta como sujeta a segundo factor obligatorio (RF-005, cuentas de funcionario).</summary>
    Task HabilitarMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una cuenta de funcionario con el rol indicado (no "Ciudadano": ese solo se obtiene por
    /// autorregistro). Solo el Administrador puede invocar esta vía — a diferencia del autorregistro
    /// público, aquí el rol se decide en el momento de la creación, no después. Habilita MFA
    /// automáticamente (RF-005: toda cuenta de funcionario exige segundo factor).
    /// </summary>
    Task<CrearUsuarioResultado> CrearUsuarioConRolAsync(
        string userName, string email, string password, string nombreRol, CancellationToken cancellationToken = default);

    /// <summary>Indica si la cuenta exige segundo factor en el inicio de sesión (RF-005).</summary>
    Task<bool> RequiereMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Genera el código de verificación de un solo uso, enviado por correo (RF-005).</summary>
    Task<string> GenerarCodigoMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Valida el código de verificación del segundo factor (RF-005).</summary>
    Task<bool> VerificarCodigoMfaAsync(Guid usuarioId, string codigo, CancellationToken cancellationToken = default);
}

public sealed record CrearUsuarioResultado(bool Succeeded, Guid UsuarioId, IReadOnlyList<string> Errores);

public sealed record ValidarCredencialesResultado(bool Succeeded, bool BloqueadoTemporalmente, Guid UsuarioId);

public sealed record UsuarioBasico(Guid Id, string UserName, string? Email = null);
