using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sitram.Infrastructure.Persistence.Cifrado;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Fábrica en tiempo de diseño para las herramientas de EF Core (migraciones), de modo que
/// <c>dotnet ef</c> no dependa del proyecto de arranque ni de su configuración. Lee la cadena de
/// conexión real desde los User Secrets de Sitram.Api (mismo <c>UserSecretsId</c> del csproj) para
/// que "migrations add" y "database update" siempre apunten a la misma base, sin importar qué
/// <c>--startup-project</c> se pase en la línea de comandos.
/// </summary>
public sealed class SitramDbContextFactory : IDesignTimeDbContextFactory<SitramDbContext>
{
    public SitramDbContext CreateDbContext(string[] args)
    {
        var configuracion = new ConfigurationBuilder()
            .AddUserSecrets("40aa4e79-a870-44c3-aff7-5ca151e5a931") // UserSecretsId de Sitram.Api
            .AddEnvironmentVariables()
            .Build();

        var cadenaConexion = configuracion.GetConnectionString("SitramDb")
            ?? "Host=localhost;Port=5432;Database=SitramDb;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<SitramDbContext>()
            .UseNpgsql(cadenaConexion)
            .Options;

        // Las migraciones solo necesitan la FORMA del modelo, nunca cifran datos reales: basta
        // una clave de relleno para construir CifradoColumna.
        var configuracionCifrado = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cifrado:Clave"] = Convert.ToBase64String(new byte[64]),
            })
            .Build();

        // Las migraciones no disparan eventos de dominio; basta un publisher inerte.
        return new SitramDbContext(options, new PublisherInerte(), new CifradoColumna(configuracionCifrado));
    }

    private sealed class PublisherInerte : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
