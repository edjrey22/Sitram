using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Almacenamiento;

/// <summary>
/// Almacena los documentos adjuntos en disco local (RF-021). El nombre físico es un Guid (evita
/// colisiones y *path traversal* a partir del nombre que envía el cliente); se calcula el
/// SHA-256 en streaming mientras se escribe, sin cargar el archivo completo en memoria.
/// </summary>
public sealed class AlmacenamientoArchivosLocal : IAlmacenamientoArchivos
{
    private readonly string _directorioBase;

    public AlmacenamientoArchivosLocal(IConfiguration configuration)
    {
        _directorioBase = configuration["Almacenamiento:DirectorioBase"]
            ?? Path.Combine(AppContext.BaseDirectory, "documentos-adjuntos");
        Directory.CreateDirectory(_directorioBase);
    }

    public async Task<(string RutaAlmacenamiento, string HashSha256)> GuardarAsync(
        string nombreArchivo, Stream contenido, CancellationToken cancellationToken = default)
    {
        var nombreFisico = $"{Guid.NewGuid():N}{Path.GetExtension(nombreArchivo)}";
        var rutaCompleta = Path.Combine(_directorioBase, nombreFisico);

        using var sha256 = SHA256.Create();
        await using (var destino = File.Create(rutaCompleta))
        await using (var cryptoStream = new CryptoStream(destino, sha256, CryptoStreamMode.Write))
        {
            await contenido.CopyToAsync(cryptoStream, cancellationToken);
        }

        var hash = Convert.ToHexStringLower(sha256.Hash!);
        return (nombreFisico, hash);
    }

    public Task<Stream> AbrirAsync(string rutaAlmacenamiento, CancellationToken cancellationToken = default)
    {
        var rutaCompleta = Path.Combine(_directorioBase, rutaAlmacenamiento);
        Stream stream = File.OpenRead(rutaCompleta);
        return Task.FromResult(stream);
    }
}
