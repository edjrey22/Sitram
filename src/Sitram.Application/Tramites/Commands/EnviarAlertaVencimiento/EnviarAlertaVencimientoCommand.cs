using MediatR;

namespace Sitram.Application.Tramites.Commands.EnviarAlertaVencimiento;

/// <summary>Notifica al ciudadano que su trámite observado está por vencer (RF-053).</summary>
public sealed record EnviarAlertaVencimientoCommand(Guid TramiteId) : IRequest;
