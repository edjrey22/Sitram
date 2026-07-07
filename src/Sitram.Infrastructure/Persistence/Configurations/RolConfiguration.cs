using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Infrastructure.Identity;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.RolId);
        builder.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.Nombre).IsUnique();
    }
}
