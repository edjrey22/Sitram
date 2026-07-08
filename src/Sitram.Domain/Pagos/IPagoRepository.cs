using Sitram.Domain.Tramites;

namespace Sitram.Domain.Pagos;

/// <summary>Puerto de persistencia del agregado <see cref="Pago"/> (implementado en Infrastructure).</summary>
public interface IPagoRepository
{
    Task AddAsync(Pago pago, CancellationToken cancellationToken = default);

    Task<Pago?> ObtenerPorIdAsync(PagoId id, CancellationToken cancellationToken = default);

    /// <summary>El pago más reciente asociado a un trámite (para verificar RF-043).</summary>
    Task<Pago?> ObtenerPorTramiteAsync(TramiteId tramiteId, CancellationToken cancellationToken = default);
}
