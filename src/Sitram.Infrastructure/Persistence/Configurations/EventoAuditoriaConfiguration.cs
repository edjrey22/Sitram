using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sitram.Infrastructure.Persistence;

namespace Sitram.Infrastructure.Persistence.Configurations;

public sealed class EventoAuditoriaConfiguration : IEntityTypeConfiguration<EventoAuditoria>
{
    public void Configure(EntityTypeBuilder<EventoAuditoria> builder)
    {
        builder.ToTable("EventosAuditoria");
        builder.HasKey(e => e.EventoId);
        builder.Property(e => e.EventoId).ValueGeneratedOnAdd();

        builder.Property(e => e.Accion).HasMaxLength(100).IsRequired();
        builder.Property(e => e.DatosAntes).HasColumnType("text");
        builder.Property(e => e.DatosDespues).HasColumnType("text");
        builder.Property(e => e.DireccionIp).HasMaxLength(45);
        builder.Property(e => e.FechaUtc).IsRequired();

        builder.HasIndex(e => e.FechaUtc);   // IX_Auditoria_Fecha (modelo-datos.md §4)
        builder.HasIndex(e => e.TramiteId);
    }
}
