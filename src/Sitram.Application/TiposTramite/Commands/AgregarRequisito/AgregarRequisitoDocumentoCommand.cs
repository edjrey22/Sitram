using MediatR;

namespace Sitram.Application.TiposTramite.Commands.AgregarRequisito;

/// <summary>Añade un documento requerido a un tipo de trámite (RF-011).</summary>
public sealed record AgregarRequisitoDocumentoCommand(int TipoTramiteId, string Nombre, bool Obligatorio) : IRequest;
