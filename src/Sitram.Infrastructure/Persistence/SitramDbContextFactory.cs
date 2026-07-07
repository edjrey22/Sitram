using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Fábrica en tiempo de diseño para las herramientas de EF Core (migraciones), de modo que
/// <c>dotnet ef</c> no dependa del proyecto de arranque ni de su configuración.
/// </summary>
public sealed class SitramDbContextFactory : IDesignTimeDbContextFactory<SitramDbContext>
{
    public SitramDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SitramDbContext>()
            .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SitramDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        // Las migraciones no disparan eventos de dominio; basta un publisher inerte.
        return new SitramDbContext(options, new PublisherInerte());
    }

    private sealed class PublisherInerte : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
