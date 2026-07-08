using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> builder)
    {
        builder.ToTable("Documentos");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.NombreArchivo).HasMaxLength(200).IsRequired();
        builder.Property(d => d.RutaAlmacenamiento).HasMaxLength(300).IsRequired();
        builder.Property(d => d.HashSha256).HasMaxLength(64).IsRequired();
        builder.Property(d => d.SubidoUtc).IsRequired();
    }
}
