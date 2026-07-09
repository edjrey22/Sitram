using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.TiposTramite;
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

        // RF-053: plazo de subsanación y control de la alerta de vencimiento.
        builder.Property(t => t.FechaLimiteSubsanacionUtc);
        builder.Property(t => t.AlertaVencimientoEnviada).IsRequired();

        // Concurrencia optimista: evita la doble aprobación (errores-conocidos 1.1, modelo-datos §5).
        // Postgres no tiene "rowversion" (específico de SQL Server): se mapea la columna de
        // sistema "xmin", que Postgres ya reescribe en cada UPDATE de la fila, como token de
        // concurrencia (patrón documentado por Npgsql para EF Core).
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasIndex(t => t.Codigo).IsUnique();   // UQ_Tramite_Codigo (correlativo público)
        builder.HasIndex(t => t.CiudadanoId);         // IX_Tramite_Ciudadano (RF-050)

        // FK real hacia TipoTramite: corrige el hueco de integridad (TipoTramiteId antes era
        // un int sin validar). Relación por sombra: Tramite no tiene navegación en el dominio,
        // los agregados solo se referencian por Id (DDD).
        builder.HasOne<TipoTramite>()
            .WithMany()
            .HasForeignKey(t => t.TipoTramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Historial de actuaciones: colección expuesta como solo lectura, mapeada por su campo de respaldo
        builder.HasMany(t => t.Historial)
            .WithOne()
            .HasForeignKey("TramiteId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Historial).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Documentos adjuntos (RF-021): mismo patrón que el historial de actuaciones.
        builder.HasMany(t => t.Documentos)
            .WithOne()
            .HasForeignKey("TramiteId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(t => t.Documentos).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
