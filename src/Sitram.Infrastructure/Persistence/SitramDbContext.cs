using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core del sistema. Cada entidad se configura con su
/// <c>IEntityTypeConfiguration&lt;T&gt;</c> en Persistence/Configurations (modelo-datos.md §6).
/// </summary>
public class SitramDbContext(DbContextOptions<SitramDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
