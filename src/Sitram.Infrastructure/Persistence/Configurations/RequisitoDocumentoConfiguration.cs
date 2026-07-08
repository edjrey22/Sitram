using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.TiposTramite;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class RequisitoDocumentoConfiguration : IEntityTypeConfiguration<RequisitoDocumento>
{
    public void Configure(EntityTypeBuilder<RequisitoDocumento> builder)
    {
        builder.ToTable("RequisitosDocumento");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(r => r.Obligatorio).IsRequired();
    }
}
