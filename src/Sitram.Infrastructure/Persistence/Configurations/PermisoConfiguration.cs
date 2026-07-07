using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Infrastructure.Identity;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos");
        builder.HasKey(p => p.PermisoId);
        builder.Property(p => p.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.Codigo).IsUnique();
    }
}
