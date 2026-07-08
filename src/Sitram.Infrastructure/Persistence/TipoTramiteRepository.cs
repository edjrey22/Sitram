using Microsoft.EntityFrameworkCore;
using Sitram.Domain.TiposTramite;

namespace Sitram.Infrastructure.Persistence;

/// <summary>Implementación del puerto <see cref="ITipoTramiteRepository"/> sobre EF Core.</summary>
public sealed class TipoTramiteRepository(SitramDbContext context) : ITipoTramiteRepository
{
    public async Task AddAsync(TipoTramite tipoTramite, CancellationToken cancellationToken = default) =>
        await context.TiposTramite.AddAsync(tipoTramite, cancellationToken);

    public async Task<TipoTramite?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.TiposTramite
            .Include(t => t.Requisitos)
            .Include(t => t.PasosFlujo)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}
