using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Seguridad;

namespace Sitram.Infrastructure.Persistence.Configurations;

/// <summary>Mapeo EF Core del agregado <see cref="IncidenteSeguridad"/> (RF-065).</summary>
public sealed class IncidenteSeguridadConfiguration : IEntityTypeConfiguration<IncidenteSeguridad>
{
    public void Configure(EntityTypeBuilder<IncidenteSeguridad> builder)
    {
        builder.ToTable("IncidentesSeguridad");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => new IncidenteSeguridadId(value))
            .ValueGeneratedNever();

        builder.Property(i => i.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(i => i.Gravedad).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(i => i.FechaDeteccionUtc).IsRequired();
        builder.Property(i => i.FechaNotificacionUtc);
        builder.Property(i => i.OficialNotificadoId);
        builder.Property(i => i.Resolucion).HasMaxLength(2000);
        builder.Property(i => i.FechaResolucionUtc);
    }
}
