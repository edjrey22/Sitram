using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Interfaces;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Infrastructure.Identity;

/// <summary>Implementación de <see cref="IIdentityService"/> sobre ASP.NET Core Identity.</summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    SitramDbContext context) : IIdentityService
{
    private const string RolPorDefecto = "Ciudadano";
    private const string RolOficialDatos = "OficialDatosPersonales";

    public async Task<CrearUsuarioResultado> CrearUsuarioAsync(
        string userName, string email, string password, CancellationToken cancellationToken = default)
    {
        var usuario = new ApplicationUser { UserName = userName, Email = email };
        var resultado = await userManager.CreateAsync(usuario, password);

        if (!resultado.Succeeded)
            return new CrearUsuarioResultado(false, Guid.Empty, resultado.Errors.Select(e => e.Description).ToList());

        var rol = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolPorDefecto, cancellationToken);
        if (rol is not null)
        {
            context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.RolId });
            await context.SaveChangesAsync(cancellationToken);
        }

        return new CrearUsuarioResultado(true, usuario.Id, []);
    }

    public async Task<ValidarCredencialesResultado> ValidarCredencialesAsync(
        string userName, string password, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByNameAsync(userName);
        if (usuario is null)
            return new ValidarCredencialesResultado(false, false, Guid.Empty);

        // lockoutOnFailure: true aplica el bloqueo tras 5 intentos fallidos (RF-003, IdentityOptions.Lockout).
        var resultado = await signInManager.CheckPasswordSignInAsync(usuario, password, lockoutOnFailure: true);

        if (resultado.IsLockedOut)
            return new ValidarCredencialesResultado(false, true, usuario.Id);

        return new ValidarCredencialesResultado(resultado.Succeeded, false, usuario.Id);
    }

    public async Task<IReadOnlyList<string>> ObtenerPermisosAsync(Guid usuarioId, CancellationToken cancellationToken = default) =>
        await (from ur in context.UsuarioRoles
               join rp in context.RolPermisos on ur.RolId equals rp.RolId
               join p in context.Permisos on rp.PermisoId equals p.PermisoId
               where ur.UsuarioId == usuarioId
               select p.Codigo)
              .Distinct()
              .ToListAsync(cancellationToken);

    public async Task<UsuarioBasico?> ObtenerUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
        return usuario is null ? null : new UsuarioBasico(usuario.Id, usuario.UserName!, usuario.Email);
    }

    public async Task<string> GenerarTokenConfirmacionEmailAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");
        return await userManager.GenerateEmailConfirmationTokenAsync(usuario);
    }

    public async Task<bool> ConfirmarEmailAsync(Guid usuarioId, string token, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
        if (usuario is null) return false;

        var resultado = await userManager.ConfirmEmailAsync(usuario, token);
        return resultado.Succeeded;
    }

    public async Task<UsuarioBasico?> ObtenerUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByEmailAsync(email);
        return usuario is null ? null : new UsuarioBasico(usuario.Id, usuario.UserName!, usuario.Email);
    }

    public async Task<string> GenerarTokenRecuperacionContrasenaAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");
        return await userManager.GeneratePasswordResetTokenAsync(usuario);
    }

    public async Task<bool> RestablecerContrasenaAsync(
        Guid usuarioId, string token, string nuevaContrasena, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
        if (usuario is null) return false;

        var resultado = await userManager.ResetPasswordAsync(usuario, token, nuevaContrasena);
        return resultado.Succeeded;
    }

    public async Task DesignarOficialDatosAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");

        var rol = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolOficialDatos, cancellationToken)
            ?? throw new InvalidOperationException($"El rol {RolOficialDatos} no está sembrado.");

        // El cargo es singular: se retira el rol a quien lo tuviera antes de asignarlo al nuevo oficial.
        var asignacionesPrevias = context.UsuarioRoles.Where(ur => ur.RolId == rol.RolId);
        context.UsuarioRoles.RemoveRange(asignacionesPrevias);
        context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.RolId });

        await context.SaveChangesAsync(cancellationToken);

        // RF-005: toda cuenta de funcionario exige segundo factor; el Oficial de Datos lo es.
        await HabilitarMfaAsync(usuario.Id, cancellationToken);
    }

    public async Task<UsuarioBasico?> ObtenerOficialDatosActivoAsync(CancellationToken cancellationToken = default)
    {
        var rol = await context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolOficialDatos, cancellationToken);
        if (rol is null) return null;

        var usuarioId = await context.UsuarioRoles
            .Where(ur => ur.RolId == rol.RolId)
            .Select(ur => (Guid?)ur.UsuarioId)
            .FirstOrDefaultAsync(cancellationToken);

        return usuarioId is null ? null : await ObtenerUsuarioAsync(usuarioId.Value, cancellationToken);
    }

    public async Task HabilitarMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");

        await userManager.SetTwoFactorEnabledAsync(usuario, true);
    }

    public async Task<bool> RequiereMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
        return usuario is not null && await userManager.GetTwoFactorEnabledAsync(usuario);
    }

    public async Task<string> GenerarCodigoMfaAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe el usuario {usuarioId}.");

        return await userManager.GenerateTwoFactorTokenAsync(usuario, TokenOptions.DefaultEmailProvider);
    }

    public async Task<bool> VerificarCodigoMfaAsync(Guid usuarioId, string codigo, CancellationToken cancellationToken = default)
    {
        var usuario = await userManager.FindByIdAsync(usuarioId.ToString());
        if (usuario is null) return false;

        return await userManager.VerifyTwoFactorTokenAsync(usuario, TokenOptions.DefaultEmailProvider, codigo);
    }
}
