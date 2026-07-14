using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Sitram.Infrastructure.Persistence.Cifrado;

/// <summary>
/// Cifrado a nivel de columna para datos personales (RNF-003), equivalente funcional a
/// SQL Server Always Encrypted: <see cref="CifrarDeterministico"/> permite búsqueda por
/// igualdad (Dni, Correo) derivando el IV del propio texto plano; <see cref="CifrarAleatorio"/>
/// usa un IV aleatorio (Teléfono, sin necesidad de búsqueda exacta). El dominio nunca ve estos
/// bytes: la conversión ocurre solo en los <c>ValueConverter</c> de Infrastructure — exactamente
/// como Always Encrypted es transparente al código de aplicación (ver ADR-0004).
/// </summary>
public sealed class CifradoColumna
{
    private readonly byte[] _claveCifrado;
    private readonly byte[] _claveMac;

    public CifradoColumna(IConfiguration configuration)
    {
        var claveBase64 = configuration["Cifrado:Clave"]
            ?? throw new InvalidOperationException("Falta configurar 'Cifrado:Clave' (User Secrets en desarrollo).");
        var clave = Convert.FromBase64String(claveBase64);
        if (clave.Length != 64)
            throw new InvalidOperationException("'Cifrado:Clave' debe representar 64 bytes en Base64 (32 AES + 32 HMAC).");

        _claveCifrado = clave[..32];
        _claveMac = clave[32..];
    }

    public byte[] CifrarDeterministico(string texto) => Cifrar(texto, DerivarIvDeterministico(texto));

    public byte[] CifrarAleatorio(string texto) => Cifrar(texto, RandomNumberGenerator.GetBytes(16));

    public string Descifrar(byte[] datos)
    {
        if (datos == null || datos.Length < 16)
            throw new FormatException("Los datos a descifrar no tienen la longitud mínima requerida.");

        var iv = datos[..16];
        var cifrado = datos[16..];

        using var aes = Aes.Create();
        aes.Key = _claveCifrado;
        aes.IV = iv;
        using var descifrador = aes.CreateDecryptor();

        try
        {
            var plano = descifrador.TransformFinalBlock(cifrado, 0, cifrado.Length);
            return Encoding.UTF8.GetString(plano);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException(
                "Error al descifrar los datos de la columna. Esto suele ocurrir si la base de datos contiene " +
                "registros creados con una 'Cifrado:Clave' distinta a la actual. Asegúrate de vaciar/recrear la " +
                "base de datos si la clave cambió (muy común en entornos de desarrollo/pruebas).", ex);
        }
    }

    private byte[] Cifrar(string texto, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = _claveCifrado;
        aes.IV = iv;
        using var cifrador = aes.CreateEncryptor();
        var planoBytes = Encoding.UTF8.GetBytes(texto);
        var cifrado = cifrador.TransformFinalBlock(planoBytes, 0, planoBytes.Length);
        return [.. iv, .. cifrado];
    }

    /// <summary>IV = HMAC-SHA256(claveMac, texto)[..16]: mismo texto -> mismo IV -> mismo cifrado.</summary>
    private byte[] DerivarIvDeterministico(string texto)
    {
        var hash = HMACSHA256.HashData(_claveMac, Encoding.UTF8.GetBytes(texto));
        return hash[..16];
    }
}
