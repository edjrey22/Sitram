using MediatR;

namespace Sitram.Application.Tramites.Commands.AdjuntarDocumento;

/// <summary>Adjunta un documento (PDF/imagen) al expediente (RF-021).</summary>
public sealed record AdjuntarDocumentoCommand(Guid TramiteId, string NombreArchivo, Stream Contenido) : IRequest<Guid>;
