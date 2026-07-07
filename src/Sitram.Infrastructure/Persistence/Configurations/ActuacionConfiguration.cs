using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence.Configurations;

/// <summary>Mapeo EF Core de la entidad hija <see cref="Actuacion"/> (historial de transiciones).</summary>
public sealed class ActuacionConfiguration : IEntityTypeConfiguration<Actuacion>
{
    public void Configure(EntityTypeBuilder<Actuacion> builder)
    {
        builder.ToTable("Actuaciones");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.EstadoAnterior).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.EstadoNuevo).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.Comentario).HasMaxLength(500);
        builder.Property(a => a.FechaUtc).IsRequired();
    }
}
