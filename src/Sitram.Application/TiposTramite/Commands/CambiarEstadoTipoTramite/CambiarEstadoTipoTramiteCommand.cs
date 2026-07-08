using MediatR;

namespace Sitram.Application.TiposTramite.Commands.CambiarEstadoTipoTramite;

/// <summary>Activa o desactiva (borrado lógico) un tipo de trámite (RF-013).</summary>
public sealed record CambiarEstadoTipoTramiteCommand(int TipoTramiteId, bool Activar) : IRequest;
