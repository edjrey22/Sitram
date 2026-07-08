using Sitram.Domain.Common;

namespace Sitram.Domain.Tramites;

/// <summary>
/// Entidad hija del agregado <see cref="Tramite"/>: documento adjunto al expediente (RF-021).
/// Se crea solo desde el agregado.
/// </summary>
public sealed class Documento : Entity<Guid>
{
    public string NombreArchivo { get; private set; } = default!;
    public string RutaAlmacenamiento { get; private set; } = default!;
    public string HashSha256 { get; private set; } = default!;
    public DateTime SubidoUtc { get; private set; }

    internal Documento(string nombreArchivo, string rutaAlmacenamiento, string hashSha256) : base(Guid.NewGuid())
    {
        NombreArchivo = nombreArchivo;
        RutaAlmacenamiento = rutaAlmacenamiento;
        HashSha256 = hashSha256;
        SubidoUtc = DateTime.UtcNow;
    }

    // Requerido por EF Core
    private Documento() { }
}
