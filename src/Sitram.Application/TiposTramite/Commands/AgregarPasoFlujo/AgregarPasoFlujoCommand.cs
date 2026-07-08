using MediatR;

namespace Sitram.Application.TiposTramite.Commands.AgregarPasoFlujo;

/// <summary>Añade un paso al flujo de aprobación de un tipo de trámite (RF-012).</summary>
public sealed record AgregarPasoFlujoCommand(int TipoTramiteId, int Orden, int RolResponsableId) : IRequest;
