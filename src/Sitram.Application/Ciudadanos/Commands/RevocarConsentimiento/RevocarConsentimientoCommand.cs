using MediatR;

namespace Sitram.Application.Ciudadanos.Commands.RevocarConsentimiento;

/// <summary>Revoca el consentimiento vigente para una finalidad (RF-064, derecho de oposición).</summary>
public sealed record RevocarConsentimientoCommand(Guid CiudadanoId, string Finalidad) : IRequest;
