using MediatR;

namespace Sitram.Application.Ciudadanos.Commands.OtorgarConsentimiento;

/// <summary>Registra el consentimiento informado para una finalidad de tratamiento (RF-063).</summary>
public sealed record OtorgarConsentimientoCommand(Guid CiudadanoId, string Finalidad) : IRequest;
