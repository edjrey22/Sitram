using MediatR;

namespace Sitram.Application.Seguridad.Commands.DesignarOficialDatos;

/// <summary>Designa y registra al Oficial de Datos Personales, con acceso a incidentes y solicitudes ARCO (RF-066).</summary>
public sealed record DesignarOficialDatosCommand(Guid UsuarioId) : IRequest;
