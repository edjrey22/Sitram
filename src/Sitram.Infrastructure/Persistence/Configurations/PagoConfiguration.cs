using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Pagos;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence.Configurations;

/// <summary>Mapeo EF Core del agregado <see cref="Pago"/> (modelo-datos.md).</summary>
public sealed class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PagoId(value))
            .ValueGeneratedNever();

        builder.Property(p => p.TramiteId)
            .HasConversion(id => id.Value, value => new TramiteId(value))
            .IsRequired();

        builder.Property(p => p.Monto).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.ReferenciaPasarela).HasMaxLength(100);
        builder.Property(p => p.FechaUtc).IsRequired();

        builder.HasIndex(p => p.TramiteId); // IX_Pago_Tramite (modelo-datos.md §4)

        builder.HasOne<Tramite>()
            .WithMany()
            .HasForeignKey(p => p.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
