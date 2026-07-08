using MediatR;

namespace Sitram.Application.Pagos.Commands.ConfirmarPago;

/// <summary>
/// Confirma el pago (simula el callback de la pasarela, modo prueba) y avanza el trámite en la
/// <b>misma transacción</b> (RF-042, RNF-032: pago + cambio de estado son atómicos).
/// </summary>
public sealed record ConfirmarPagoCommand(Guid PagoId) : IRequest;
