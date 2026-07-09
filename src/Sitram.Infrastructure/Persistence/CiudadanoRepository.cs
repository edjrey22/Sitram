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

    // El converter de Dni (cifrado determinista) se aplica también al parámetro de la consulta,
    // así que esta comparación se traduce a buscar por igualdad de bytes cifrados en SQL.
    public async Task<Ciudadano?> ObtenerPorDniAsync(Dni dni, CancellationToken cancellationToken = default) =>
        await context.Ciudadanos.FirstOrDefaultAsync(c => c.Dni == dni, cancellationToken);
}
