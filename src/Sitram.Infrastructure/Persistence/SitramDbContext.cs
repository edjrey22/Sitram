using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Events;
using Sitram.Application.Common.Interfaces;
using Sitram.Domain.Ciudadanos;
using Sitram.Domain.Common;
using Sitram.Domain.Pagos;
using Sitram.Domain.TiposTramite;
using Sitram.Domain.Tramites;
using Sitram.Infrastructure.Identity;
using Sitram.Infrastructure.Persistence.Cifrado;
using Sitram.Infrastructure.Persistence.Configurations;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core del sistema y adaptador de <see cref="IUnitOfWork"/>. Extiende
/// <see cref="IdentityUserContext{TUser,TKey}"/> (usuarios de Identity, sin sus tablas de rol:
/// el RBAC usa las tablas propias Rol/Permiso/UsuarioRol/RolPermiso de modelo-datos.md).
/// Cada entidad se configura con su <c>IEntityTypeConfiguration&lt;T&gt;</c> en
/// Persistence/Configurations.
/// </summary>
public class SitramDbContext(DbContextOptions<SitramDbContext> options, IPublisher publisher, CifradoColumna cifrado)
    : IdentityUserContext<ApplicationUser, Guid>(options), IUnitOfWork
{
    public DbSet<Tramite> Tramites => Set<Tramite>();
    public DbSet<TipoTramite> TiposTramite => Set<TipoTramite>();
    public DbSet<Ciudadano> Ciudadanos => Set<Ciudadano>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EventoAuditoria> EventosAuditoria => Set<EventoAuditoria>();

    /// <summary>
    /// Guarda los cambios y, si tienen éxito, despacha los eventos de dominio acumulados en los
    /// agregados rastreados (p. ej. auditoría de cada transición de <see cref="Tramite"/>, RF-070).
    /// </summary>
    public async Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        var entidadesConEventos = ChangeTracker.Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var eventos = entidadesConEventos.SelectMany(e => e.DomainEvents).ToList();

        var resultado = await SaveChangesAsync(cancellationToken);

        foreach (var entidad in entidadesConEventos)
            entidad.ClearDomainEvents();

        foreach (var evento in eventos)
        {
            var tipoNotificacion = typeof(DomainEventNotification<>).MakeGenericType(evento.GetType());
            var notificacion = Activator.CreateInstance(tipoNotificacion, evento)!;
            await publisher.Publish(notificacion, cancellationToken);
        }

        return resultado;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CiudadanoConfiguration se excluye del escaneo automático porque requiere CifradoColumna
        // por constructor (cifra Dni/Correo/Telefono); se aplica manualmente a continuación.
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(), t => t != typeof(CiudadanoConfiguration));
        modelBuilder.ApplyConfiguration(new CiudadanoConfiguration(cifrado));
    }
}
