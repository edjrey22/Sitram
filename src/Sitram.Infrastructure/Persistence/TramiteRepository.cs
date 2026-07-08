using Microsoft.EntityFrameworkCore;
using Sitram.Domain.Tramites;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación del puerto <see cref="ITramiteRepository"/> sobre EF Core.</summary>
public sealed class TramiteRepository(SitramDbContext context) : ITramiteRepository
{
    public async Task AddAsync(Tramite tramite, CancellationToken cancellationToken = default) =>
        await context.Tramites.AddAsync(tramite, cancellationToken);

    public async Task<Tramite?> ObtenerPorIdAsync(TramiteId id, CancellationToken cancellationToken = default) =>
        await context.Tramites
            .Include(t => t.Historial)
            .Include(t => t.Documentos)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}
