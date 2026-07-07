namespace Sitram.Application.Common.Interfaces;

/// <summary>Puerto para la integración con la pasarela de pagos (RF-041, en modo prueba).</summary>
public interface IPagoService
{
    Task<string> RegistrarPagoAsync(decimal monto, string referencia, CancellationToken cancellationToken = default);
}
