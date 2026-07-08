using Sitram.Application.Common.Interfaces;

namespace Sitram.Infrastructure.Persistence;

/// <summary>
/// Adaptador de la pasarela de pagos en <b>modo prueba</b> (RF-W01, fuera de alcance la
/// integración bancaria real): genera una referencia simulada sin llamar a ningún servicio externo.
/// </summary>
public sealed class PagoService : IPagoService
{
    public Task<string> RegistrarPagoAsync(decimal monto, string referencia, CancellationToken cancellationToken = default) =>
        Task.FromResult($"SIMULADO-{referencia}-{DateTime.UtcNow:yyyyMMddHHmmss}");
}
