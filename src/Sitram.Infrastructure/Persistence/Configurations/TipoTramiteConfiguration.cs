using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.TiposTramite;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class TipoTramiteConfiguration : IEntityTypeConfiguration<TipoTramite>
{
    public void Configure(EntityTypeBuilder<TipoTramite> builder)
    {
        builder.ToTable("TiposTramite");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Descripcion).HasMaxLength(500);
        builder.Property(t => t.AreaResponsable).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Tasa).HasColumnType("decimal(10,2)");
        builder.Property(t => t.Activo).IsRequired();

        builder.HasMany(t => t.Requisitos)
            .WithOne()
            .HasForeignKey("TipoTramiteId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Requisitos).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.PasosFlujo)
            .WithOne()
            .HasForeignKey("TipoTramiteId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.PasosFlujo).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
