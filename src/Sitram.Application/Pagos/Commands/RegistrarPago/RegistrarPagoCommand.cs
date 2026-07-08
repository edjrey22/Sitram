using MediatR;

namespace Sitram.Application.Pagos.Commands.RegistrarPago;

/// <summary>Registra el pago de la tasa de un trámite, calculada desde su tipo (RF-040, RF-041).</summary>
public sealed record RegistrarPagoCommand(Guid TramiteId) : IRequest<RegistrarPagoResultado>;

public sealed record RegistrarPagoResultado(Guid PagoId, decimal Monto, string ReferenciaPasarela);
