using Microsoft.EntityFrameworkCore;
using Sitram.Domain.Pagos;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación del puerto <see cref="IPagoRepository"/> sobre EF Core.</summary>
public sealed class PagoRepository(SitramDbContext context) : IPagoRepository
{
    public async Task AddAsync(Pago pago, CancellationToken cancellationToken = default) =>
        await context.Pagos.AddAsync(pago, cancellationToken);

    public async Task<Pago?> ObtenerPorIdAsync(PagoId id, CancellationToken cancellationToken = default) =>
        await context.Pagos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Pago?> ObtenerPorTramiteAsync(TramiteId tramiteId, CancellationToken cancellationToken = default) =>
        await context.Pagos
            .Where(p => p.TramiteId == tramiteId)
            .OrderByDescending(p => p.FechaUtc)
            .FirstOrDefaultAsync(cancellationToken);
}
