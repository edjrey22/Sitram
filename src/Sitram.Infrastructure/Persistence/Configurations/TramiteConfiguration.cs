using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence.Configurations;

/// <summary>Mapeo EF Core del agregado <see cref="Tramite"/> (modelo-datos.md §4, §5).</summary>
public sealed class TramiteConfiguration : IEntityTypeConfiguration<Tramite>
{
    public void Configure(EntityTypeBuilder<Tramite> builder)
    {
        builder.ToTable("Tramites");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TramiteId(value))
            .ValueGeneratedNever();

        builder.Property(t => t.CiudadanoId).IsRequired();
        builder.Property(t => t.TipoTramiteId).IsRequired();

        builder.Property(t => t.Estado)
            .HasConversion<string>()      // se almacena como texto legible
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(t => t.CreadoUtc).IsRequired();

        // Concurrencia optimista: evita la doble aprobación (errores-conocidos 1.1, modelo-datos §5).
        // Propiedad sombra: no contamina el dominio.
        builder.Property<byte[]>("RowVersion").IsRowVersion();

        builder.HasIndex(t => t.Codigo).IsUnique();   // UQ_Tramite_Codigo (correlativo público)
        builder.HasIndex(t => t.CiudadanoId);         // IX_Tramite_Ciudadano (RF-050)

        // Historial de actuaciones: colección expuesta como solo lectura, mapeada por su campo de respaldo
        builder.HasMany(t => t.Historial)
            .WithOne()
            .HasForeignKey("TramiteId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Historial).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
