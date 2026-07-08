using Microsoft.EntityFrameworkCore;
using Sitram.Domain.Ciudadanos;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación del puerto <see cref="ICiudadanoRepository"/> sobre EF Core.</summary>
public sealed class CiudadanoRepository(SitramDbContext context) : ICiudadanoRepository
{
    public async Task AddAsync(Ciudadano ciudadano, CancellationToken cancellationToken = default) =>
        await context.Ciudadanos.AddAsync(ciudadano, cancellationToken);

    public async Task<Ciudadano?> ObtenerPorIdAsync(CiudadanoId id, CancellationToken cancellationToken = default) =>
        await context.Ciudadanos
            .Include(c => c.Consentimientos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
