using Sitram.Domain.Common;
using Sitram.Domain.Tramites;

namespace Sitram.Domain.Pagos.Events;

/// <summary>Se confirmó el pago de la tasa de un trámite (RF-042; auditoría RF-070).</summary>
public sealed record PagoConfirmadoEvent(PagoId PagoId, TramiteId TramiteId) : IDomainEvent;
