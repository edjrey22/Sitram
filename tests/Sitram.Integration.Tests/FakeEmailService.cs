using Sitram.Application.Common.Interfaces;

namespace Sitram.Integration.Tests;

/// <summary>
/// Reemplaza a <c>EmailService</c> en las pruebas de integración: registra los envíos en
/// memoria en vez de intentar una conexión SMTP real.
/// </summary>
public sealed class FakeEmailService : IEmailService
{
    public List<(string Destino, string Asunto, string Cuerpo)> Enviados { get; } = new();

    public Task EnviarAsync(string destino, string asunto, string cuerpo, CancellationToken cancellationToken = default)
    {
        lock (Enviados) Enviados.Add((destino, asunto, cuerpo));
        return Task.CompletedTask;
    }
}
