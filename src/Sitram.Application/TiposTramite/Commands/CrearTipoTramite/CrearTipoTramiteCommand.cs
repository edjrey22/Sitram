using MediatR;

namespace Sitram.Application.TiposTramite.Commands.CrearTipoTramite;

/// <summary>Crea un tipo de trámite del TUPA, activo por defecto (RF-010).</summary>
public sealed record CrearTipoTramiteCommand(string Nombre, string Descripcion, string AreaResponsable, decimal Tasa)
    : IRequest<int>;
