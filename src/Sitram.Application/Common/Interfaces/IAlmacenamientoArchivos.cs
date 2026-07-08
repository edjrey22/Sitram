namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto de almacenamiento de archivos adjuntos (RF-021). Implementado en Infrastructure.</summary>
public interface IAlmacenamientoArchivos
{
    /// <summary>Guarda el contenido y devuelve la ruta de almacenamiento y su hash SHA-256 (integridad).</summary>
    Task<(string RutaAlmacenamiento, string HashSha256)> GuardarAsync(
        string nombreArchivo, Stream contenido, CancellationToken cancellationToken = default);

    Task<Stream> AbrirAsync(string rutaAlmacenamiento, CancellationToken cancellationToken = default);
}
