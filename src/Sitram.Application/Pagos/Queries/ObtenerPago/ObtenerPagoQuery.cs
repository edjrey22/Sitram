using MediatR;

namespace Sitram.Application.Pagos.Queries.ObtenerPago;

/// <summary>Consulta el estado de un pago.</summary>
public sealed record ObtenerPagoQuery(Guid PagoId) : IRequest<PagoDto?>;

public sealed record PagoDto(
    Guid Id, Guid TramiteId, decimal Monto, string Estado, string? ReferenciaPasarela, DateTime FechaUtc);
