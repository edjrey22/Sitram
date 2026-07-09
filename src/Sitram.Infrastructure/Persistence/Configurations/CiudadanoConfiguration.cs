using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Ciudadanos;
using Sitram.Infrastructure.Persistence.Cifrado;

namespace Sitram.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeo EF Core del agregado <see cref="Ciudadano"/> (modelo-datos.md). Se instancia
/// explícitamente en <see cref="SitramDbContext.OnModelCreating"/> (no vía
/// <c>ApplyConfigurationsFromAssembly</c>) porque necesita <see cref="CifradoColumna"/> por
/// constructor para cifrar/descifrar Dni/Correo/Telefono de forma transparente al dominio.
/// </summary>
public sealed class CiudadanoConfiguration(CifradoColumna cifrado) : IEntityTypeConfiguration<Ciudadano>
{
    public void Configure(EntityTypeBuilder<Ciudadano> builder)
    {
        builder.ToTable("Ciudadanos");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => new CiudadanoId(value))
            .ValueGeneratedNever();

        builder.Property(c => c.Nombres).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Apellidos).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Direccion).HasMaxLength(200).IsRequired(); // protegida por TDE (nivel BD), no a nivel columna

        // Cifrado determinista (Dni, Correo): permite índice único / búsqueda por igualdad.
        builder.Property(c => c.Dni)
            .HasConversion(dni => cifrado.CifrarDeterministico(dni.Valor), bytes => new Dni(cifrado.Descifrar(bytes)))
            .HasColumnType("bytea")
            .IsRequired();
        builder.HasIndex(c => c.Dni).IsUnique();

        builder.Property(c => c.Correo)
            .HasConversion(correo => cifrado.CifrarDeterministico(correo), bytes => cifrado.Descifrar(bytes))
            .HasColumnType("bytea")
            .IsRequired();

        // Cifrado aleatorio (Telefono): no requiere búsqueda por igualdad.
        builder.Property(c => c.Telefono)
            .HasConversion(tel => cifrado.CifrarAleatorio(tel), bytes => cifrado.Descifrar(bytes))
            .HasColumnType("bytea")
            .IsRequired();

        builder.Property(c => c.EstaAnonimizado).IsRequired();
        builder.Property(c => c.CreadoUtc).IsRequired();

        builder.HasMany(c => c.Consentimientos)
            .WithOne()
            .HasForeignKey("CiudadanoId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Consentimientos).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
