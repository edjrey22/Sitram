using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.TiposTramite;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class PasoFlujoConfiguration : IEntityTypeConfiguration<PasoFlujo>
{
    public void Configure(EntityTypeBuilder<PasoFlujo> builder)
    {
        builder.ToTable("PasosFlujo");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Orden).IsRequired();
        builder.Property(p => p.RolResponsableId).IsRequired();
    }
}
