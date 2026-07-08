using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class ConsentimientoConfiguration : IEntityTypeConfiguration<Consentimiento>
{
    public void Configure(EntityTypeBuilder<Consentimiento> builder)
    {
        builder.ToTable("Consentimientos");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Finalidad).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Otorgado).IsRequired();
        builder.Property(c => c.FechaUtc).IsRequired();
    }
}
